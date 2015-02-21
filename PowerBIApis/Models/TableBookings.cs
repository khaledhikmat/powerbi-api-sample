using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis.Models
{
    public class TableBookings
    {
        public DateTime TransactionDate { get; set; }
        public int Bookings { get; set; }
        public int Books { get; set; }
        public int Canx { get; set; }
        public int Confirmations { get; set; }
        public int NoShows { get; set; }
    }
}
