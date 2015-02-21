using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis.Models
{
    public class Sales
    {
        public DateTime TransactionDate { get; set; }
        public int Pens { get; set; }
        public int Books { get; set; }
        public int Calculators { get; set; }
    }
}
