using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.Models
{
    public class GISRecord
    {
        public GISRecord()
        {
            Information = new Dictionary<string, string>();
            MatchingWECC = new List<Tuple<GISRecord, double>>();
            MatchingEnergyAnalytics = new List<Tuple<GISRecord, double>>();
            MatchingPlatts = new List<Tuple<GISRecord, double>>();
            Location = new LatLngPoint();
        }
        public string Source { get; set; }
        //public double Latitude { get; set; }
        //public double Longitude { get; set; }
        public string BusNumber { get; set; }
        public string BusName { get; set; }
        public string SubstationNumber { get; set; }
        public string SubstationName { get; set; }
        public LatLngPoint Location { get; set; }
        public string Description
        {
            get
            {
                return string.Join(Environment.NewLine, Information.Select(x => x.Key + " = " + x.Value));
            }
        }
        public Dictionary<string, string> Information { get; set; }

        public List<Tuple<GISRecord, double>> MatchingWECC { get; set; }
        public List<Tuple<GISRecord, double>> MatchingEnergyAnalytics { get; set; }
        public List<Tuple<GISRecord, double>> MatchingPlatts { get; set; }
        public GMapMarker Marker { get; set; }
    }
}
