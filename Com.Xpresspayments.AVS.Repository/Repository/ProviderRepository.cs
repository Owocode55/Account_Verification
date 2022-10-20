using Com.Xpresspayments.AVS.Data.Model;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Repository
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly IDapper _dapper;

        public ProviderRepository(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<List<Provider>> GetAllProviders()
        {
            var clientList = await _dapper.GetAll<Provider>("[dbo].[sp_GetAllProviders]", null, commandType: CommandType.StoredProcedure);
            return clientList;
        }

        public Provider UpdateProvider(Provider provider)
        {
            var dbPara = new DynamicParameters();
            dbPara.Add("TokenExpiryDate", provider.TokenExpiryDate);
            dbPara.Add("AccessToken", provider.AccessToken, DbType.String);
            dbPara.Add("ProviderID", provider.ID);
            var Provider = _dapper.Update<Provider>("[dbo].[sp_UpdateProvider]", dbPara, commandType: CommandType.StoredProcedure);
            return Provider;
        }
    }
}
