using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GISData.Model
{
    public class GISRecord
    {
        public GISRecord()
        {
            Information = new Dictionary<string, string>();
            MatchingWECC = new List<GISRecord>();
            MatchingEnergyAnalytics = new List<GISRecord>();
            MatchingPlatts = new List<GISRecord>();
        }
        public string Source { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string BusNumber { get; set; }
        public string BusName { get; set; }
        public string SubstationNumber { get; set; }
        public string SubstationName { get; set; }
        public string Description
        {
            get
            {
                return string.Join(Environment.NewLine, Information.Select(x => x.Key + " = " + x.Value));
            }
        }
        public Dictionary<string, string> Information { get; set; }

        public List<GISRecord> MatchingWECC { get; set; }
        public List<GISRecord> MatchingEnergyAnalytics { get; set; }
        public List<GISRecord> MatchingPlatts { get; set; }
    }
}
