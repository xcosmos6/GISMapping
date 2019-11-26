using GISData.Model;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelReader
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
                while (!parser.EndOfData)
                {
                    //Processing row
                    var newRecord = new GISRecord();
                    newRecord.Source = source;
                    string[] fields = parser.ReadFields();
                    for (int i = 0; i < fields.Count(); i++)
                    {
                        newRecord.Information[headers[i]] = fields[i];
                        if (headers[i] == "Latitude")
                        {
                            double lat;
                            if (double.TryParse(fields[i], out lat))
                            {
                                newRecord.Latitude = lat;
                            }
                        }
                        if (headers[i] == "Longitude")
                        {
                            double lng;
                            if (double.TryParse(fields[i], out lng))
                            {
                                newRecord.Longitude = lng;
                            }
                        }
                        if (headers[i] == "BUS-NO" || headers[i] == "busnum") //wecc || energyanalyics from wecc?
                        {
                            newRecord.BusNumber = fields[i];
                        }
                        if (headers[i] == "NAME" || headers[i] == "Name") //wecc || energyanalyics
                        {
                            newRecord.BusName = fields[i].ToLower();
                        }
                        if (headers[i] == "SUBSTATION NUMBER" || headers[i] == "busnum") //wecc || energyanalyics from wecc?
                        {
                            newRecord.SubstationNumber = fields[i];
                        }
                        if (headers[i] == "SUBSTATION NAME" || headers[i] == "SUB_NAME") //wecc || Platts
                        {
                            newRecord.SubstationName = fields[i].ToLower();
                        }
                    }
                    rslts.Add(newRecord);
                }
            }
            return rslts;
        }
    }
}
