using Com.Xpresspayments.AVS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{
    public class GenericResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public AVSDetails Data { get; set; }
    }

    public class AVSDetails
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }

    public class ProviderList
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public List<Provider> Data { get; set; }

    }
}
