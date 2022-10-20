using AutoMapper;
using Com.Xpresspayments.AVS.Common.Utiities;
using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Com.Xpresspayments.AVS.Data.ViewModel.SterlingBankDTO;

namespace Com.Xpresspayments.AVS.Services.Integrations
{
    public class SterlingBank : IFactory
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _loggerManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string baseUrl, endpoint, sharekey, vectorKey, requestType, appId, translocation, interEndPoint, requestTypeNEInter;

        public SterlingBank(IMapper mapper, IConfiguration config, IHttpClientFactory httpClientFactory, ILoggerManager loggerManager)
        {
            _mapper = mapper;
            _config = config;
            _loggerManager = loggerManager;
            _httpClientFactory = httpClientFactory;
            //baseUrl = _config["SterlingBank:BaseUrl"];
           // endpoint = _config["SterlingBank:endPoint"];
            //interEndPoint = _config["SterlingBank:interEndPoint"];
            sharekey = _config["SterlingBank:SharedKey"];
            vectorKey = _config["SterlingBank:SharedVector"];
            requestType = _config["SterlingBank:RequestTypeNE"];
            requestTypeNEInter = _config["SterlingBank:RequestTypeNEInter"];
            appId = _config["SterlingBank:AppId"];
            translocation = _config["SterlingBank:Translocation"];
        }

        public async Task<GenericProviderResponse> InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            GenericProviderResponse genericProviderResponse = new();
            var urlbuilder = $"{provider.BaseURL}/{provider.IntraBankMethod}";
            Random rnd = new Random();
            string nextRnd = DateTime.Now.ToString("yyMMddHHmmss") + rnd.Next(9999);

            SPBInterBankNameEnq interBankNameEnq = new SPBInterBankNameEnq()
            {
                DestinationBankCode = accountNameVM.BankCode,
                Referenceid = "NE" + nextRnd,
                ToAccount = accountNameVM.AccountNumber,
                RequestType = Convert.ToInt32(requestTypeNEInter),
                Translocation = translocation
            };

            string encryptedString = TrippleDesEncrypt(JsonConvert.SerializeObject(interBankNameEnq), sharekey, vectorKey);
            StringContent stringContent = new StringContent(encryptedString, Encoding.UTF8, "text/plain");

            var httpClient = _httpClientFactory.CreateClient("Sterling");
            httpClient.DefaultRequestHeaders.Add("AppId", appId);
            //httpClient.BaseAddress = new Uri(provider.BaseURL);
            //var response = await httpClient.PostAsync(provider.InterBankMethod, stringContent);
            var response = await httpClient.PostAsync(urlbuilder, stringContent);
            string content = response.Content.ReadAsStringAsync().Result;
            _loggerManager.LogInformation("Response From Stearling Inter Bank Name Enquiry ------------- " + content);
            var enquiryRes = JsonConvert.DeserializeObject<SterlingBankResponse>(content);

            if (enquiryRes?.data?.status == "00")
            {
               genericProviderResponse.AccountName = enquiryRes.data.AccountName;
               genericProviderResponse.ResponseCode = "00";
            }
            else
            {
               genericProviderResponse.AccountName = enquiryRes?.data?.AccountName;
               genericProviderResponse.ResponseCode = "05";
            }
            return genericProviderResponse;
        }

        public async Task<GenericProviderResponse> IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            GenericProviderResponse genericProviderResponse = new();
            var urlbuilder = $"{provider.BaseURL}/{provider.IntraBankMethod}";
            Random rnd = new Random();
            string nextRnd = DateTime.Now.ToString("yyMMddHHmmss") + rnd.Next(9999);
            SBPNameEnquiryRequest nameEnquiryRequest = new ()
            {
                NUBAN = accountNameVM.AccountNumber,
                Referenceid = "NE"+ nextRnd,
                RequestType = Convert.ToInt32(requestType),
                Translocation = translocation
            };
            string encryptedString = TrippleDesEncrypt(JsonConvert.SerializeObject(nameEnquiryRequest), sharekey, vectorKey);
            StringContent stringContent = new StringContent(encryptedString, Encoding.UTF8, "text/plain");

            var httpClient = _httpClientFactory.CreateClient("Sterling");
            httpClient.DefaultRequestHeaders.Add("AppId", appId);
            //httpClient.BaseAddress = new Uri(L);
            var response = await httpClient.PostAsync(urlbuilder, stringContent);
            string content = response.Content.ReadAsStringAsync().Result;
            _loggerManager.LogInformation("Response From Stearling Intra Bank Name Enquiry ------------- " + content);
            var enquiryRes = JsonConvert.DeserializeObject<SterlingBankResponse>(content);

            if (enquiryRes?.data?.status == "00")
            {
               genericProviderResponse.AccountName = enquiryRes.data.AccountName;
               genericProviderResponse.ResponseCode = "00";
            }
            else
            {
               genericProviderResponse.AccountName = enquiryRes?.data?.AccountName;
               genericProviderResponse.ResponseCode = "05";
            }

            return genericProviderResponse;
        }

    public  string BinaryToString(string binary)
    {
        if (string.IsNullOrEmpty(binary))
            throw new ArgumentNullException("binary");

        if ((binary.Length % 8) != 0)
            throw new ArgumentException("Binary string invalid (must divide by 8)", "binary");

        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < binary.Length; i += 8)
        {
            string section = binary.Substring(i, 8);
            int ascii = 0;
            try
            {
                ascii = Convert.ToInt32(section, 2);
            }
            catch
            {
                throw new ArgumentException("Binary string contains invalid section: " + section, "binary");
            }
            builder.Append((char)ascii);
        }
        return builder.ToString();
    }
    public  string TrippleDesEncrypt(string val, string sharedkeyval, string sharedvectorval)
    {
        MemoryStream ms = new();
        try
        {
            //string sharedkeyval = "";
            //string sharedvectorval = "";

            sharedkeyval = BinaryToString(sharedkeyval);

            sharedvectorval = BinaryToString(sharedvectorval);
            byte[] sharedkey = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedkeyval);
            byte[] sharedvector = System.Text.Encoding.GetEncoding("utf-8").GetBytes(sharedvectorval);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toEncrypt = Encoding.UTF8.GetBytes(val);

            CryptoStream cs = new CryptoStream(ms, tdes.CreateEncryptor(sharedkey, sharedvector), CryptoStreamMode.Write);
            cs.Write(toEncrypt, 0, toEncrypt.Length);
            cs.FlushFinalBlock();
        }
        catch
        {
            return "";
        }
        return Convert.ToBase64String(ms.ToArray());
    }
    public  string TrippleDesDecrypt(string val, string key)
    {
        TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
        MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

        byte[] byteHash;
        byte[] byteBuff;

        byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
        desCryptoProvider.Key = byteHash;
        desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB

        byteBuff = Convert.FromBase64String(val);
        string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        return plaintext;
    }
}
}