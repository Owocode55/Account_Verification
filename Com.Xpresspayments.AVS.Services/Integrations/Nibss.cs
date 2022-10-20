using AutoMapper;
using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Com.Xpresspayments.AVS.Common.Utiities;
using System.Xml;

namespace Com.Xpresspayments.AVS.Services.Integrations
{
    public class Nibss : IFactory
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _loggerManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public Nibss(IMapper mapper, IConfiguration config, IHttpClientFactory httpClientFactory , ILoggerManager loggerManager)
        {
            _mapper = mapper;
            _config = config;
            _loggerManager = loggerManager;
            _httpClientFactory = httpClientFactory;
        }
        private async Task<string> EncryptData(string data, string privateKey, string publicKey , string encryptionUrl)
        {
            try
            {
                //var resp = XpressPaymentNipLibrary.NIP.EncryptData(data, privateKey, publicKey);
                var client = _httpClientFactory.CreateClient();
                var url = $"{encryptionUrl}{data}";
                using var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ToString());
                }
                
                var encryptResp = JsonConvert.DeserializeObject<NibssEncryptionResponse>(content);
                return encryptResp.Data;
                //
                //return resp;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        private async Task<string> DecryptData(string data, string privateKey , string publicKey , string secretekey , string encryptionUrl)
        {
            //var resp = XpressPaymentNipLibrary.NIP.DecryptData(data, privateKey, publicKey, secretekey);
            var client = _httpClientFactory.CreateClient();
            var url = $"{encryptionUrl}{data}";
            using var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(response.ToString());
            }
            var encryptResp = JsonConvert.DeserializeObject<NibssEncryptionResponse>(content);
            _loggerManager.LogInformation("Decrypted Response From Nibss ------------- " + encryptResp.Data);
            return encryptResp.Data;
            //
            // return resp;
        }

        private string GenerateRequestXML(NESingleRequest data)
        {
            XDocument xml = new XDocument(new XDeclaration("1.0", "utf-8", null),
            new XElement("NESingleRequest",
            new XElement("SessionID", data.SessionID),
            new XElement("DestinationInstitutionCode", data.DestinationInstitutionCode.Trim()),
            new XElement("ChannelCode", data.ChannelCode.Trim()),
            new XElement("AccountNumber", data.AccountNumber.Trim())));
            var xmlString = new StringWriter();
            xml.Save(xmlString);
            return xmlString.ToString();
        }

        private NESingleResponse GenerateResponseObjectFromXML(string response)
        {
            XDocument respXML = XDocument.Parse(response);
            var data = from b in respXML.Descendants("NESingleResponse")
                       select new NESingleResponse
                       {
                           AccountName = (string)respXML.Descendants("AccountName").First(),
                           AccountNumber = (string)respXML.Descendants("AccountNumber").First(),
                           DestinationInstitutionCode = (string)respXML.Descendants("DestinationInstitutionCode").First(),
                           SessionID = (string)respXML.Descendants("SessionID").First(),
                           BankVerificationNumber = respXML.Descendants("BankVerificationNumber").Any() ? (string)respXML.Descendants("BankVerificationNumber").First() : string.Empty,
                           ChannelCode = (string)respXML.Descendants("ChannelCode").First(),
                           KYCLevel = (string)respXML.Descendants("KYCLevel").First(),
                           ResponseCode = (string)respXML.Descendants("ResponseCode").First().Value
                       };
            //    ResponseCode = (string)respXML.Descendants("ResponseCode").First().Value
            //XDocument respXML = XDocument.Parse(response);
            //var respObject = new NESingleResponse
            //{
            //    AccountName = (string)respXML.Descendants("AccountName").First().Value,
            //    AccountNumber = (string)respXML.Descendants("AccountNumber").First().Value,
            //    DestinationInstitutionCode = (string)respXML.Descendants("DestinationInstitutionCode").First().Value,
            //    SessionID = (string)respXML.Descendants("SessionID").First().Value,
            //    BankVerificationNumber =  (string)respXML.Descendants("BankVerificationNumber").First(),
            //    ChannelCode = (string)respXML.Descendants("ChannelCode").First().Value ,
            //    KYCLevel = (string)respXML.Descendants("KYCLevel").First().Value,
            //    ResponseCode = (string)respXML.Descendants("ResponseCode").First().Value
            //};
            //return respObject;
            return data.FirstOrDefault();
        }

        private string GenerateSessionId(string institutionCode)
        {
            var date = DateTime.Now;
            //var random = new Random().Next(1000, 9999).ToString();
            return institutionCode + date.ToString("yyMMddHHmmss") + date.Ticks.ToString().Substring(date.Ticks.ToString().Length - 12);
        }
        public async Task<GenericProviderResponse> InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            var nibssConfig = new NibssConfig();
            _config.GetSection("NibssConfig").Bind(nibssConfig);
            var sessionId = GenerateSessionId(nibssConfig.InstututionCode);
            var nipClient = new NIPNameEnquiry.NIPInterfaceClient();

            var requestObject = new NESingleRequest
            {
                DestinationInstitutionCode = accountNameVM.BankCode,
                SessionID = sessionId,
                AccountNumber = accountNameVM.AccountNumber,
                ChannelCode = nibssConfig.ChannelCode
            };

            var requestXMl = GenerateRequestXML(requestObject);
            var encryptedRequest = await EncryptData(requestXMl ,nibssConfig.XpressPrivateKey , nibssConfig.NibssPublicKey , nibssConfig.EncryptURL);
            var resp  = await nipClient.nameenquirysingleitemAsync(encryptedRequest);
            if (resp == null || resp.Body == null)
                throw new Exception("NIP Body Response Is Empty");
            var decryptedResp = await DecryptData(resp.Body.@return, nibssConfig.XpressPrivateKey, nibssConfig.NibssPublicKey, nibssConfig.SecreteKey , nibssConfig.DecryptURL);
            var respObject = GenerateResponseObjectFromXML(decryptedResp);
            respObject.ResponseCode = respObject.ResponseCode == "00" ? "00" : "05";
            var respMap = _mapper.Map<GenericProviderResponse>(respObject);
            return respMap;
        }

        public async Task<GenericProviderResponse> IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            throw new NotImplementedException();
        }
    }
}
