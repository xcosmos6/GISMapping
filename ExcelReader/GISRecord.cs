using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
{
    public class GISRecord
    {
        public GISRecord()
        {
            Information = new Dictionary<string, string>();
        }
        public string Source { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Description 
        {
            get 
            {
                return string.Join(Environment.NewLine, Information.Select(x => x.Key + " = " + x.Value)); 
            } 
        }
        public Dictionary<string, string> Information { get; set; }        
    }
}
