using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class OFXFile
    {
        public OFXFile()
        {
            this.Headers = new Dictionary<string, string>();
            this.Transactions = new List<OFXFileTransaction>();
        }

        public Dictionary<string, string> Headers { get; set; }

        public string BankId { get; set; }

        public string AccountId { get; set; }

        public string AccountType { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public ICollection<OFXFileTransaction> Transactions { get; set; }
    }
}
