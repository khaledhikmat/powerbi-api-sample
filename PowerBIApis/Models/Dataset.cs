using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis.Models
{
    public class Dataset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Table> Tables { get; set; }
    }
}
