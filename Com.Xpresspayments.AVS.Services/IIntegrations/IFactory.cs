using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Services
{
    public interface IFactory
    {
        Task<GenericProviderResponse> InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider);
        Task<GenericProviderResponse> IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider);
    }
}
