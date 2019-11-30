using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapService.Models
{
    public class GISRecordWithMarker
    {
        public GISRecord Record { get; set; }
        public GMapMarker marker { get; set; }
    }
}
