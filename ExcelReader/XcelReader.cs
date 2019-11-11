using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelReader
{
    public class XcelReader
    {
        public List<GISRecord> Read(string fileName)
        {
            var rslts = new List<GISRecord>();
            //Create COM Objects. Create a COM object for everything that is referenced
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fileName);
            var numberOfSheets = xlWorkbook.Worksheets.Count;
            for (int s = 1; s <= numberOfSheets; s++)
            {
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[s];
                var sheetName = xlWorksheet.Name;
                Excel.Range xlRange = xlWorksheet.UsedRange;
                var numberofcolumns = xlRange.Columns.Count;
                var numberofRowss = xlRange.Rows.Count;
                for (int r = 2; r <= numberofRowss; r++)
                {
                    var newRecord = new GISRecord();
                    newRecord.Source = sheetName;
                    for (int c = 1; c <= numberofcolumns; c++)
                    {
                        var key = xlRange.Cells[1, c].Text;
                        var value = xlRange.Cells[r, c].Text;
                        newRecord.Information[key] = value;
                        if (key == "Latitude")
                        {
                            double lat;
                            if (double.TryParse(value, out lat))
                            {
                                newRecord.Latitude = lat;
                            }
                        }
                        if (key == "Longitude")
                        {
                            double lng;
                            if (double.TryParse(value, out lng))
                            {
                                newRecord.Longitude = lng;
                            }
                        }
                    }
                    rslts.Add(newRecord);
                }
            }
            //foreach (var item in xlRange.Columns)
            //{
            //    Console.WriteLine(item.ToString());
            //}
            return rslts;
        }
    }
}
