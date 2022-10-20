using Com.Xpresspayments.AVS.Common;
using Com.Xpresspayments.AVS.Common.Utiities;
using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Core
{
    public class InputValidationCore
    {
        private readonly AccountValidationResponses _accountResponses;

        public InputValidationCore(AccountValidationResponses accountResponses)
        {
            _accountResponses = accountResponses;
        }
        public GenericResponse ValidateAccountDetails(AccountValidationVM accountValidationVM , List<Client> clientList , string ipAddress , string appId ,out string defaultAppProvider)
        {
            defaultAppProvider = null;
            var resp = new GenericResponse();

            var applicationDetails = clientList.FirstOrDefault(a => a.AppKey == appId);
            if (applicationDetails == null)
            {
                resp = _accountResponses._respDictionary["04"];
                return resp;
            }
            defaultAppProvider = applicationDetails.DefautProvider;
            if (applicationDetails.EnableAllowedIP)
            {
                if (!applicationDetails.AllowedIP.Contains(ipAddress))
                {
                    resp = _accountResponses._respDictionary["1001"];
                    return resp;
                }
            }
            if (string.IsNullOrWhiteSpace(accountValidationVM.AccountNumber))
            {
                resp = _accountResponses._respDictionary["01"];
                return resp;
            }
            else if (string.IsNullOrWhiteSpace(accountValidationVM.BankCode))
            {
                resp = _accountResponses._respDictionary["03"];
                return resp;
            }
            else if (!NubanCheckUtil.isNuban(accountValidationVM.BankCode , accountValidationVM.AccountNumber))
            {
                resp = _accountResponses._respDictionary["01"];
                return resp;
            }
            return resp;
        }
    }
}
