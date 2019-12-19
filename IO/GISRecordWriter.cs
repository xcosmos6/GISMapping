using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapService.Models;

namespace IO
{
    public class GISRecordWriter
    {
        public void WriteCSVFilePlain(string filename, List<GISRecord> records)
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(String.Join(",", records.FirstOrDefault().Information.Keys));
                foreach (var record in records)
                {
                    var self = String.Join(",", record.Information.Values);
                    outputFile.WriteLine(self);
                }
            }
        }

        public void WriteCSVFileMatched(string filename, List<GISRecord> records, CompareOperatorEnum selectedCompareSign, string description)
        {
            int filter;
            if (int.TryParse(description, out filter))
            {
                if (selectedCompareSign == CompareOperatorEnum.GreaterThan)
                {
                    _writeRecordsGreaterThanFilter(filename, records, filter);
                }
                if (selectedCompareSign == CompareOperatorEnum.LessOrEqual)
                {
                    _writeRecordsLessOrEqThanFilter(filename, records, filter);
                }
            }
            //using (StreamWriter outputFile = new StreamWriter(filename))
            //{
            //    outputFile.WriteLine(String.Join(",", "Source", "BusNumber", "BusName", "SubstationNumber", "SubstationName", "Latitude", "Longitude", "Information"));
            //    foreach (var record in records)
            //    {
            //        var self = String.Join(",", record.Source, record.BusNumber, record.BusName, record.SubstationNumber, record.SubstationName, record.Location.Lat, record.Location.Lng, record.Description);
            //        outputFile.WriteLine(self);
            //        if (record.MatchingWECC.Count() > 0)
            //        {
            //            //self += ",,,matched WECC,";
            //            foreach (var m in record.MatchingWECC)
            //            {
            //                self += ",,distance:," + m.Item2.ToString() + "," + String.Join(",", m.Item1.Information.Values) + ",";
            //            }
            //        }
            //        if (record.MatchingEnergyAnalytics.Count() > 0)
            //        {
            //            self += ",,,matched Energy Analytics,";
            //            foreach (var m in record.MatchingEnergyAnalytics)
            //            {
            //                self += ",,distance:," + m.Item2.ToString() + "," + String.Join(",", m.Item1.Information.Values);
            //            }
            //        }
            //        if (record.MatchingPlatts.Count() > 0)
            //        {
            //            self += ",,,matched Platts,";
            //            foreach (var m in record.MatchingPlatts)
            //            {
            //                self += ",,distance:," + m.Item2.ToString() + "," + String.Join(",", m.Item1.Information.Values) + ",,,,,,";
            //            }
            //        }
            //        outputFile.WriteLine(self);
            //    }
            //}
        }

        private void _writeRecordsLessOrEqThanFilter(string filename, List<GISRecord> records, int filter)
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(String.Join(",", "Distance", "Source", "BusNumber", "BusName", "SubstationNumber", "SubstationName", "Latitude", "Longitude", "Information"));
                char[] charsToTrim = { '\'', ' ', '\"' };
                foreach (var record in records)
                {
                    var self = String.Join(",", "", record.Source, record.BusNumber, record.BusName, record.SubstationNumber, record.SubstationName, record.Location.Lat, record.Location.Lng, "\"" + string.Join(", ", record.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\"");
                    outputFile.WriteLine(self);
                    if (record.MatchingWECC.Count() > 0)
                    {
                        foreach (var m in record.MatchingWECC)
                        {
                            if (m.Item2 <= filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    if (record.MatchingEnergyAnalytics.Count() > 0)
                    {
                        foreach (var m in record.MatchingEnergyAnalytics)
                        {
                            if (m.Item2 <= filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    if (record.MatchingPlatts.Count() > 0)
                    {
                        foreach (var m in record.MatchingPlatts)
                        {
                            if (m.Item2 <= filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    outputFile.WriteLine(Environment.NewLine);
                }
            }
        }

        private void _writeRecordsGreaterThanFilter(string filename, List<GISRecord> records, int filter)
        {
            using (StreamWriter outputFile = new StreamWriter(filename))
            {
                outputFile.WriteLine(String.Join(",", "Distance", "Source", "BusNumber", "BusName", "SubstationNumber", "SubstationName", "Latitude", "Longitude", "Information"));
                char[] charsToTrim = { '\'', ' ', '\"' };
                foreach (var record in records)
                {
                    var self = String.Join(",", "", record.Source, record.BusNumber, record.BusName, record.SubstationNumber, record.SubstationName, record.Location.Lat, record.Location.Lng, "\"" + string.Join(", ", record.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\"");
                    outputFile.WriteLine(self);
                    if (record.MatchingWECC.Count() > 0)
                    {
                        foreach (var m in record.MatchingWECC)
                        {
                            if (m.Item2 > filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    if (record.MatchingEnergyAnalytics.Count() > 0)
                    {
                        foreach (var m in record.MatchingEnergyAnalytics)
                        {
                            if (m.Item2 > filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    if (record.MatchingPlatts.Count() > 0)
                    {
                        foreach (var m in record.MatchingPlatts)
                        {
                            if (m.Item2 > filter)
                            {
                                outputFile.WriteLine(String.Join(",", m.Item2, m.Item1.Source, m.Item1.BusNumber, m.Item1.BusName, m.Item1.SubstationNumber, m.Item1.SubstationName, m.Item1.Location.Lat, m.Item1.Location.Lng, "\"" + string.Join(", ", m.Item1.Information.Select(x => x.Key + " = " + x.Value.Trim(charsToTrim))) + "\""));
                            }
                        }
                    }
                    outputFile.WriteLine(Environment.NewLine);
                }
            }
        }
    }
}
