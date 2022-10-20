using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{
   public class FidelityRequestVM
    {
        public string DestinationAccount { get; set; }
        public string DestinationBankCode { get; set; }
    }

    public class FidelityResponseVM
    {
        public string ResponseCode { get; set; }
        public string AccountName { get; set; }
        public string ChannelReference { get; set; }
        public string BankVerificationNumber { get; set; }
    }
}
