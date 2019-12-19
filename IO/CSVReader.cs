using MapService.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO
{
    public class CSVReader
    {
        public List<GISRecord> Read(string filename, string source)
        {
            var rslts = new List<GISRecord>();
            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                string[] headers = parser.ReadFields();
                char[] charsToTrim = { '*', '#', '\'', ' ', '+', '\"' };
                while (!parser.EndOfData)
                {
                    //Processing row
                    var newRecord = new GISRecord();
                    newRecord.Source = source;
                    string[] fields = parser.ReadFields();
                    for (int i = 0; i < fields.Count(); i++)
                    {
                        newRecord.Information[headers[i]] = fields[i].Contains(",") ? "\"" + fields[i] + "\"" : fields[i];
                        if (headers[i] == "Latitude")
                        {
                            double lat;
                            if (double.TryParse(fields[i], out lat))
                            {
                                newRecord.Location.Lat = lat;
                            }
                        }
                        if (headers[i] == "Longitude")
                        {
                            double lng;
                            if (double.TryParse(fields[i], out lng))
                            {
                                newRecord.Location.Lng = lng;
                            }
                        }
                        if (headers[i] == "BUS-NO" || headers[i] == "busnum") //wecc || energyanalyics from wecc?
                        {
                            newRecord.BusNumber = fields[i];
                        }
                        if (headers[i] == "NAME" || headers[i] == "Name") //wecc || energyanalyics
                        {
                            var trimmedName = fields[i].ToLower().Trim(charsToTrim).Replace('_', ' ');
                            if (trimmedName.EndsWith(" (tap)"))
                            {
                                trimmedName = trimmedName.Substring(0, trimmedName.LastIndexOf(" (tap)"));
                            }
                            if (trimmedName.EndsWith(" tp"))
                            {
                                trimmedName = trimmedName.Substring(0, trimmedName.LastIndexOf(" tp"));
                            }
                            newRecord.BusName = trimmedName.Contains(",") ? "\"" + trimmedName + "\"" : trimmedName;
                            newRecord.Information["Trimmed Bus Name"] = newRecord.BusName;
                        }
                        if (headers[i] == "SUBSTATION NUMBER" || headers[i] == "busnum") //wecc || energyanalyics from wecc?
                        {
                            newRecord.SubstationNumber = fields[i];
                        }
                        if (headers[i] == "SUBSTATION NAME" || headers[i] == "SUB_NAME") //wecc || Platts
                        {
                            var trimmedName = fields[i].ToLower().Trim(charsToTrim).Replace('_', ' ');
                            if (trimmedName.EndsWith(" (tap)"))
                            {
                                trimmedName = trimmedName.Substring(0, trimmedName.LastIndexOf(" (tap)"));
                            }
                            if (trimmedName.EndsWith(" tp"))
                            {
                                trimmedName = trimmedName.Substring(0, trimmedName.LastIndexOf(" tp"));
                            }
                            newRecord.SubstationName = trimmedName.Contains(",") ? "\"" + trimmedName + "\"" : trimmedName;
                            newRecord.Information["Trimmed Substation Name"] = newRecord.SubstationName;
                        }
                    }
                    rslts.Add(newRecord);
                }
            }
            return rslts;
        }
    }
}
