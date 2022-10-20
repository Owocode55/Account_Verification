using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{

    public class UBAConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ApplCode { get; set; }
        public string ClientNo { get; set; }
        public string AccessCode { get; set; }
        public string Token { get; set; }
    }
    public class UBANameEnquiryOthersRequest
    {
        public string receiverBankCode { get; set; }
        public string receiverAccountNumber { get; set; }
        public string countryCode { get; set; }
        public string tranType { get; set; }
    }

    public class UBANameEnquiryRequest
    {
        public string receiverBankCode { get; set; }
        public string receiverAccountNumber { get; set; }
        public string countryCode { get; set; }
        public string tranType { get; set; }
    }


    public class UBANameEnquiryOthersResponse
    {
        public string ErrorFlag { get; set; }
        [DisplayName("ResponseCode")]
        public string StatusCode { get; set; }
        public string NESessionID { get; set; }
        [DisplayName("ResponseMessage")]
        public string StatusMsg { get; set; }
        [DisplayName("AccountName")]
        public string CustomerName { get; set; }
        public string BVN { get; set; }
        public string KycCode { get; set; }
    }

    public class UBANameEnquiryResponse
    {
        public string ErrorFlag { get; set; }
        [DisplayName("ResponseMessage")]
        public string StatusMsg { get; set; }
        [DisplayName("ResponseCode")]
        public string StatusCode { get; set; }
        [DisplayName("AccountName")]
        public string CustomerName { get; set; }
    }


}
