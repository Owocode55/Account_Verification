using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Repository
{
    public interface IAVSRepository
    {
        Task<AccountName> FetchAccountDetailsByAccountNumber(AccountValidationVM accountValidationVM);
        void InsertAccountDetails(AccountName details);
        void UpdateAccountDetails(AccountName details);
    }
}
