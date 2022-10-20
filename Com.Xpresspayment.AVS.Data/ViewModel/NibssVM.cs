using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{
    public class NibssConfig
    {
        public string XpressPrivateKey { get; set; }
        public string NibssPublicKey { get; set; }
        public string SecreteKey { get; set; }
        public string InstututionCode { get; set; }
        public string ChannelCode { get; set; }
        public string EncryptURL { get; set; }
        public string DecryptURL { get; set; }
    }


    public class NibssEncryptionResponse
    {
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
        public string Data { get; set; }
    }

    public class NESingleResponse
    {
        public string SessionID { get; set; }
        public string DestinationInstitutionCode { get; set; }
        public string ChannelCode { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string BankVerificationNumber { get; set; }
        public string KYCLevel { get; set; }
        public string ResponseCode { get; set; }
    }


    public class NESingleRequest
    {
        public string SessionID { get; set; }
        public string DestinationInstitutionCode { get; set; }
        public string ChannelCode { get; set; }
        public string AccountNumber { get; set; }
    }
}
