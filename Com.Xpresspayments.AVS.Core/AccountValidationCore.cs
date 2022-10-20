using Com.Xpresspayments.AVS.Common;
using Com.Xpresspayments.AVS.Common.Utiities;
using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using Com.Xpresspayments.AVS.Repository;
using Com.Xpresspayments.AVS.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Core
{
    public class AccountValidationCore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IClientRepository _clientRepository;
        private readonly IAVSRepository _avsRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IConfiguration _config;
        private readonly AccountValidationResponses _accountResponses;
        private readonly InputValidationCore _inputValidation;
        private readonly FactoryImplemetation _factoryImplemetation;

        public AccountValidationCore(IMemoryCache memoryCache, IProviderRepository providerRepository, IConfiguration config,
                                        IClientRepository clientRepository, IAVSRepository avsRepository, ILoggerManager loggerManager,
                                        AccountValidationResponses accountResponses , InputValidationCore inputValidation,
                                        FactoryImplemetation factoryImplemetation)
        {
            _config = config;
            this._memoryCache = memoryCache;
            _clientRepository = clientRepository;
            _avsRepository = avsRepository;
            _providerRepository = providerRepository;
            _accountResponses = accountResponses;
            _inputValidation = inputValidation;
            _factoryImplemetation = factoryImplemetation;
            _loggerManager = loggerManager;

        }
        public async Task<GenericResponse> ProcessAccountValidation(AccountValidationVM accountValidationVM , string appId , string ipAddress)
        {
            bool enableDefault = false;
            var resp = new GenericResponse();
            var responseDetails = new AVSDetails();
            var clientList = new List<Client>();
            try
            {
                
                clientList = CacheUtility.GetAppDetailsCache(_memoryCache, "ClientListCache");
                if (clientList == null)
                {
                    var catcheExpiryMinute = Convert.ToInt32(_config["CacheExpiryMinute"]);
                    clientList = await _clientRepository.GetAllClients();
                    CacheUtility.SetAppDetailsCache(_memoryCache, clientList, "ClientListCache" , catcheExpiryMinute);
                }

                var providerList = new List<Provider>();
                providerList = CacheUtility.GetProviderCache(_memoryCache, "ProviderListCache");
                if (providerList == null)
                {
                    var catcheExpiryMinute = Convert.ToInt32(_config["CacheExpiryMinute"]);
                    providerList = await _providerRepository.GetAllProviders();
                    CacheUtility.SetProviderCache(_memoryCache, providerList, "ProviderListCache" , catcheExpiryMinute);
                }
                string appDefaultProvider = string.Empty;
                var validationResp = _inputValidation.ValidateAccountDetails(accountValidationVM, clientList, ipAddress, appId , out appDefaultProvider);

                if (validationResp != null && validationResp.ResponseCode != null)
                    return validationResp;

                var accountDetails = await _avsRepository.FetchAccountDetailsByAccountNumber(accountValidationVM);

                if (accountDetails == null && accountValidationVM.TryCount <= 1)
                {
                    var provider = providerList.FirstOrDefault(x => x.BankCode == accountValidationVM.BankCode);

                    if(provider == null)
                    {
                        string defaultProvider = string.IsNullOrWhiteSpace(appDefaultProvider) ? _config.GetValue<string>("DefaultProvider") : appDefaultProvider;
                        provider = providerList.FirstOrDefault(x => x.BankCode == defaultProvider);
                        enableDefault = true;

                        if (provider == null)
                            throw new ApplicationException($"Default provider with bank code {defaultProvider} not found");
                    }
                    IFactory processor = _factoryImplemetation.FactoryConsumer(provider.Providername);
                    var processorResponse = enableDefault ? await processor.InterBankNameValidation(accountValidationVM, provider) :
                              await processor.IntraBankNameValidation(accountValidationVM, provider);

                    //LogRespones
                    if(processorResponse.ResponseCode == "00")
                    {
                        var accountName = new AccountName
                        {
                            AccountNumber = accountValidationVM.AccountNumber,
                            BankCode = accountValidationVM.BankCode,
                            DataCreated = DateTime.Now,
                            CreatedBy = "System",
                            Name = processorResponse.AccountName
                        };
                        _avsRepository.InsertAccountDetails(accountName);
                    }

                    resp = _accountResponses._respDictionary[processorResponse.ResponseCode];
                    responseDetails.AccountName = processorResponse.AccountName;
                    responseDetails.AccountNumber = accountValidationVM.AccountNumber;
                    resp.Data = responseDetails;
                }
                else if(accountValidationVM.TryCount > 1)
                {
                    Provider provider = null;
                    var minimumAvsValidityDays = Convert.ToDouble(_config.GetValue<string>("MinimumAVSValidityDays"));

                    if (accountDetails != null)
                    {
                        var dateCreatedOrUpdated = accountDetails.DateUpdated == null ? accountDetails.DataCreated : accountDetails.DateUpdated;
                        if((DateTime.Now - Convert.ToDateTime(dateCreatedOrUpdated)).TotalDays <= minimumAvsValidityDays)
                        {
                            resp = _accountResponses._respDictionary["00"];
                            responseDetails.AccountName = accountDetails.Name;
                            responseDetails.AccountNumber = accountDetails.AccountNumber;
                            resp.Data = responseDetails;
                            return resp;
                        }

                        if (accountValidationVM.TryCount <= 2)
                        {
                            provider = providerList.FirstOrDefault(x => x.BankCode == accountValidationVM.BankCode);
                        }
                    }else if(accountValidationVM.TryCount <= 2)
                    {

                    }

                    if(provider == null)
                    {
                        var providersWithOutInterBank = _config.GetValue<string>("ProvidersWithOutInterbank");
                        var excepted = providersWithOutInterBank.Split(",");

                        providerList = (from o in providerList
                                        join p in excepted on o.BankCode equals p into t
                                        from od in t.DefaultIfEmpty()
                                        where od == null
                                        select o).ToList();

                        Random rnd = new Random();
                        int randomInt = rnd.Next(providerList.Count);
                        provider = providerList[randomInt];
                    }
                    
                    IFactory processor = _factoryImplemetation.FactoryConsumer(provider.Providername);
                    var processorResponse = accountValidationVM.BankCode != provider.BankCode ? await processor.InterBankNameValidation(accountValidationVM, provider) :
                              await processor.IntraBankNameValidation(accountValidationVM, provider);

                    if (processorResponse.ResponseCode == "00")
                    {
                        var accountName = new AccountName
                        {
                            AccountNumber = accountValidationVM.AccountNumber,
                            BankCode = accountValidationVM.BankCode,
                            DataCreated = DateTime.Now,
                            CreatedBy = "System",
                            Name = processorResponse.AccountName,
                            DateUpdated = DateTime.Now
                        };
                        if (accountDetails == null)
                            _avsRepository.InsertAccountDetails(accountName);
                        else
                            _avsRepository.UpdateAccountDetails(accountName);
                    }
                    resp = _accountResponses._respDictionary[processorResponse.ResponseCode];
                    responseDetails.AccountName = processorResponse.AccountName;
                    responseDetails.AccountNumber = accountValidationVM.AccountNumber;
                    resp.Data = responseDetails;
                }
                else
                {
                    resp = _accountResponses._respDictionary["00"];
                    responseDetails.AccountName = accountDetails.Name;
                    responseDetails.AccountNumber = accountDetails.AccountNumber;
                    resp.Data = responseDetails;
                }

            }catch(Exception ex)
            {
                _loggerManager.LogError(new {IPaddress = ipAddress ,AccountNumber = accountValidationVM.AccountNumber , accountValidationVM .BankCode , ApplicationId = appId}, ex);
                resp = _accountResponses._respDictionary["10"];
                //resp.ResponseMessage = resp.ResponseMessage + ex.Message.Substring(0 , 100);
            }
            return resp;
        }


        public Provider GetDefaultProvider(string appDefaultProvider , List<Provider> providerList)
        {
            string defaultProvider = string.IsNullOrWhiteSpace(appDefaultProvider) ? _config.GetValue<string>("DefaultProvider") : appDefaultProvider;
            var provider = providerList.FirstOrDefault(x => x.BankCode == defaultProvider);

            if (provider == null)
                throw new ApplicationException($"Default provider with bank code {defaultProvider} not found");
            return provider;
        }

        public async Task<ProviderList> GetAllProviders(string appId , string ipAddress)
        {
            var providerList = new List<Provider>();
            var resp = new ProviderList();

            var clientList = CacheUtility.GetAppDetailsCache(_memoryCache, "ClientListCache");
            if (clientList == null)
            {
                var catcheExpiryMinute = Convert.ToInt32(_config["CacheExpiryMinute"]);
                clientList = await _clientRepository.GetAllClients();
                CacheUtility.SetAppDetailsCache(_memoryCache, clientList, "ClientListCache", catcheExpiryMinute);
            }

            var applicationDetails = clientList.FirstOrDefault(a => a.AppKey == appId);
            if (applicationDetails == null)
            {
                resp.ResponseCode = _accountResponses._respDictionary["04"].ResponseCode;
                resp.ResponseMessage = _accountResponses._respDictionary["04"].ResponseMessage;
                return resp;
            }

            if (applicationDetails.EnableAllowedIP)
            {
                if (!applicationDetails.AllowedIP.Contains(ipAddress))
                {
                    resp.ResponseCode = _accountResponses._respDictionary["1001"].ResponseCode;
                    resp.ResponseMessage = _accountResponses._respDictionary["1001"].ResponseMessage;
                    return resp;
                }
            }

            providerList = CacheUtility.GetProviderCache(_memoryCache, "ProviderListCache");
            if (providerList == null)
            {
                var catcheExpiryMinute = Convert.ToInt32(_config["CacheExpiryMinute"]);
                providerList = await _providerRepository.GetAllProviders();
                CacheUtility.SetProviderCache(_memoryCache, providerList, "ProviderListCache", catcheExpiryMinute);
            }
            resp.ResponseCode = _accountResponses._respDictionary["00"].ResponseCode;
            resp.ResponseMessage = _accountResponses._respDictionary["00"].ResponseMessage;
            resp.Data = providerList;

            return resp;
        }


    }
}
