using AutoMapper;
using Com.Xpresspayments.AVS.Common.Utiities;
using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Services.Integrations
{
    public class UBA : IFactory
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILoggerManager _loggerManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public UBA(IMapper mapper, IConfiguration config, IHttpClientFactory httpClientFactory, ILoggerManager loggerManager)
        {
            _mapper = mapper;
            _config = config;
            _loggerManager = loggerManager;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<GenericProviderResponse> InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            var ubaConfig = new UBAConfig();
            _config.GetSection("UBAConfig").Bind(ubaConfig);

            HttpClient httpClient = _httpClientFactory.CreateClient("UBA");
            var urlbuilder = $"{provider.BaseURL}/{provider.InterBankMethod}";
            var byteArray = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ubaConfig.UserName}:{ubaConfig.Password}"));
            var bearer = "Bearer " + ubaConfig.AccessCode;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", byteArray);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("AccessCode", bearer);
            httpClient.DefaultRequestHeaders.Add("Applcode", ubaConfig.ApplCode);
            httpClient.DefaultRequestHeaders.Add("Clientno", ubaConfig.ClientNo);
            var json = JsonConvert.SerializeObject(new UBANameEnquiryOthersRequest
            {
                countryCode = "NG",
                receiverAccountNumber = accountNameVM.AccountNumber,
                receiverBankCode = accountNameVM.BankCode,
                tranType = "nameenquiry"
            });

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync(urlbuilder, data);
            var content = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode)
            {
                throw new Exception(resp.ToString());
            }
            _loggerManager.LogInformation("Response From UBA Inter Bank Name Enquiry ------------- " + content);
            var ubaResp = JsonConvert.DeserializeObject<UBANameEnquiryOthersResponse>(content);
            ubaResp.StatusCode = ubaResp.ErrorFlag.ToLower() == "false" ? "00" : "05";
            ubaResp.StatusMsg = ubaResp.ErrorFlag.ToLower() == "false" ? "Successful" : "Error Finding Details - " + ubaResp.StatusMsg;
            var respMap = _mapper.Map<GenericProviderResponse>(ubaResp);

            return respMap;
        }

        public async Task<GenericProviderResponse> IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            var ubaConfig = new UBAConfig();
            _config.GetSection("UBAConfig").Bind(ubaConfig);
            HttpClient httpClient = _httpClientFactory.CreateClient("UBA");
            var urlbuilder = $"{provider.BaseURL}/{provider.IntraBankMethod}";
            var byteArray =  "Basic "+Convert.ToBase64String(Encoding.ASCII.GetBytes($"{ubaConfig.UserName}:{ubaConfig.Password}"));
            var bearer = "Bearer " + ubaConfig.AccessCode ;
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", byteArray);
            
            httpClient.DefaultRequestHeaders.Add("AccessCode",bearer);
            httpClient.DefaultRequestHeaders.Add("Applcode",ubaConfig.ApplCode );
            httpClient.DefaultRequestHeaders.Add("Clientno",ubaConfig.ClientNo );
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var json = JsonConvert.SerializeObject(new UBANameEnquiryRequest
            {
                countryCode = "NG",
                receiverAccountNumber = accountNameVM.AccountNumber,
                receiverBankCode = accountNameVM.BankCode,
                tranType = "nameenquiry"
            });

            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await httpClient.PostAsync(urlbuilder, data);
            var content = await resp.Content.ReadAsStringAsync();
            _loggerManager.LogInformation("Response From UBA Intra Bank Name Enquiry ------------- " + content);
            var ubaResp = JsonConvert.DeserializeObject<UBANameEnquiryResponse>(content);
            ubaResp.StatusCode = ubaResp.ErrorFlag.ToLower() == "false" ? "00" : "05";
            ubaResp.StatusMsg = ubaResp.ErrorFlag.ToLower() == "false" ? "Successful" : "Error Finding Details";
            var respMap = _mapper.Map<GenericProviderResponse>(ubaResp);

            return respMap;
        }

       
    }
}
