using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using NETGeographicLib;
//using GeographicLib_SHARED;

namespace MapService.Models
{
    public class LatLngPoint
    {
        public double Lat { get; set; }
        public double Lng { get; set; }

        public LatLngPoint(PointLatLng position)
        {
            Lat = position.Lat;
            Lng = position.Lng;
        }

        internal double GetDistance(LatLngPoint point2)
        {
            var geod = new Geodesic(); // WGS84
            double s12, azi1, azi2;
            var a12 = geod.Inverse(Lat, Lng, point2.Lat, point2.Lng, out s12, out azi1, out azi2);
            return s12;
        }

        internal PointLatLng GetPointLatLng()
        {
            return new PointLatLng(Lat, Lng);
        }
    }
}
