using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.ViewModel
{
    public class AccountValidationVM
    {
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public int TryCount { get; set; }
    }
}
