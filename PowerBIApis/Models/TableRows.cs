using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis.Models
{
    public class TableRows<T>
    {
        public TableRows()
        {
            Rows = new List<T>();
        }

        public List<T> Rows { get; set; }
    }
}
