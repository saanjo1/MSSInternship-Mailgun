using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailguncApp.Models
{
    public class Warranty
    {
        public string WarrantyID { get; set; }
        public string AssetID { get; set; }
        public double PriceOfWarranty { get; set; }
        public DateTime Duration { get; set; }
        public string ContactMail { get; set; }
        public bool PossibilityToExtend { get; set; }
    }
}
