using Com.Xpresspayments.AVS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Repository
{
    public interface IProviderRepository
    {
        Task<List<Provider>> GetAllProviders();
        Provider UpdateProvider(Provider provider);
    }
}
