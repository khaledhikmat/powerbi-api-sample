using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBIApis.Models
{
    public class DatasetsList
    {
        public DatasetsList()
        {
            Datasets = new List<Dataset>();
        }

        public List<Dataset> Datasets { get; set; }
    }
}
