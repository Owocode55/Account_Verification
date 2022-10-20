using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.Data.Model
{
    
    [Table("Clients")]
    public class Client
    {
        
        public int ID { get; set; }
        public string AppName { get; set; }
        public string AppKey { get; set; }
        public bool EnableAllowedIP { get; set; }
        public bool Active { get; set; }
        public string AllowedIP { get; set; }
        public string DefautProvider { get; set; }

    }
}
