using Com.Xpresspayments.AVS.Data.Model;
using Com.Xpresspayments.AVS.Data.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Repository
{
    public class AVSRepository : IAVSRepository
    {
        private readonly IDapper _dapper;

        public AVSRepository(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<AccountName> FetchAccountDetailsByAccountNumber(AccountValidationVM accountValidationVM)
        {
            var dbPara = new DynamicParameters();
            dbPara.Add("AccountNumber", accountValidationVM.AccountNumber);
            dbPara.Add("BankCode", accountValidationVM.BankCode, DbType.String);

            var accountDetails = await _dapper.Get<AccountName>("[dbo].[sp_GetAccountDetails]", dbPara, commandType: CommandType.StoredProcedure);
            return accountDetails;
        }

        public async void InsertAccountDetails(AccountName details)
        {
            var dbPara = new DynamicParameters();
            dbPara.Add("Name", details.Name);
            dbPara.Add("AccountNumber", details.AccountNumber);
            dbPara.Add("BankCode", details.BankCode, DbType.String);
            dbPara.Add("CreatedBy", details.CreatedBy);
            dbPara.Add("DataCreated", details.DataCreated.ToString("yyyy-MM-dd HH:mm:ss.fff") , DbType.DateTime);

            var accountDetails = await _dapper.Insert<AccountName>("[dbo].[sp_InsertAccountDetail]", dbPara, commandType: CommandType.StoredProcedure);
            //return accountDetails;
        }

        public async void UpdateAccountDetails(AccountName details)
        {
            var dbPara = new DynamicParameters();
            dbPara.Add("Name", details.Name);
            dbPara.Add("AccountNumber", details.AccountNumber);
            dbPara.Add("BankCode", details.BankCode, DbType.String);
            dbPara.Add("DataUpdated", ((DateTime)details.DateUpdated).ToString("yyyy-MM-dd HH:mm:ss.fff"), DbType.DateTime);

            var accountDetails = await _dapper.Insert<AccountName>("[dbo].[sp_UpdateAccountDetail]", dbPara, commandType: CommandType.StoredProcedure);
            //return accountDetails;
        }
    }
}
