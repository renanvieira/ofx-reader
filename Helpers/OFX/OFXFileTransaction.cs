using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class OFXFileTransaction
    {

        public string Type { get; set; }

        public DateTime DatePosted { get; set; }

        public decimal Amount { get; set; }

        public string FitId { get; set; }
                
        public string Memo { get; set; }

        public string CheckNum { get; set; }

        public string UniqueId
        {
            get
            {
                string data = string.Format("{0}|{1}|{2}|{3}", this.Type, this.DatePosted, this.Amount, this.Memo);
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
            }
        }
    }
}
