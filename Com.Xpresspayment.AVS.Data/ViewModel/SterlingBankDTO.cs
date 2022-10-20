using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{
    public class SterlingBankDTO
    {
        public class SPBInterBankNameEnq
        {
            public string Referenceid { get; set; }
            public int RequestType { get; set; }
            public string Translocation { get; set; }
            public string ToAccount { get; set; }
            public string DestinationBankCode { get; set; }
    }
        public class SBPNameEnquiryRequest
        {
            public string Referenceid { get; set; }
            public int RequestType { get; set; }
            public string Translocation { get; set; }
            public string NUBAN { get; set; }
        }

        public class SterlingBankResponse
      {
        public string message { get; set; }
        public string response { get; set; }
        public object Responsedata { get; set; }
        public Data data { get; set; }
      }
       
    public class Data
       {
            public string AccountName { get; set; }
            public string AccountNumber { get; set; }
            public string status { get; set; }
        }
    }
}
