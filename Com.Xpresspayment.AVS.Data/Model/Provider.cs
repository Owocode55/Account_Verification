using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.Model
{
    [Table("Provider")]
    public class Provider
    {
        public int ID { get; set; }
        public string Providername { get; set; }
        public string BankCode { get; set; }
        public string BaseURL { get; set; }
        public string AuthMethod { get; set; }
        public string InterBankMethod { get; set; }
        public string IntraBankMethod { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AccessToken { get; set; }
        public DateTime TokenExpiryDate { get; set; }
        public string DateCreated { get; set; }
        public string Createdby { get; set; }
    }
}
