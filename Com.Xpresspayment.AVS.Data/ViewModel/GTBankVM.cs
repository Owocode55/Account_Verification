using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{

    public class GTBankConfig
    {
        public string AccessCode { get; set; }
        public string UserName { get; set; }
        public string AdminUserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class GTNameValidationResponse
    {
        public string message { get; set; }
        public GTNameEnquiryDetails Data { get; set; }
    }

    public class GTAuthResponseVM
    {
        public string Message { get; set; }
        public BrearerTokenVM Data { get; set; }
    }
    
    public class BrearerTokenVM
    {
        public string BearerToken { get; set; }
        public DateTime ExpiryTime { get; set; }
    }

    public class GTNameEnquiryDetails
    {
        public string AccountName { get; set; }
        public string currencyCode { get; set; }
        public string currencyDesc { get; set; }
        public string ResponseCode { get; set; }
    }




}
