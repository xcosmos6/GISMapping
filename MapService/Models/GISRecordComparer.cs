using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET.WindowsPresentation;
using MapService.ViewModels;

namespace MapService.Models
{
    public class GISRecordComparer
    {
        public List<GISRecord> WECCGISRecords { get; set; }
        private List<GISRecord> _plattsGISRecords;
        public List<GISRecord> PlattsGISRecords
        {
            get { return _plattsGISRecords; }
            set
            {
                _plattsGISRecords = value;
                if (value != null)
                {
                    _plattsDict = _plattsGISRecords.GroupBy(x => x.SubstationName).ToDictionary(x => x.Key, x => x.ToList());
                }
            }
        }
        private List<GISRecord> _energyAnalyticsGISRecords;
        public List<GISRecord> ENERGYANAGISRecords
        {
            get { return _energyAnalyticsGISRecords; }
            set
            {
                _energyAnalyticsGISRecords = value;
                if (value != null)
                {
                    _eaDict = _energyAnalyticsGISRecords.GroupBy(x => x.BusName).ToDictionary(x => x.Key, x => x.ToList());
                    //_eaBusNumberDict = _energyAnalyticsGISRecords.GroupBy(x => x.BusNumber).ToDictionary(x => x.Key, x => x.ToList());
                }
            }
        }
        private Dictionary<string, List<GISRecord>> _eaDict;
        private Dictionary<string, List<GISRecord>> _eaBusNumberDict;
        private Dictionary<string, List<GISRecord>> _plattsDict;
        //public void MathWECCWithEnergyAnalytics()
        //{
        //    if (WECCGISRecords != null)
        //    {
        //        foreach (var record in WECCGISRecords)
        //        {
        //            var key = record.SubstationName;
        //            if (_eaDict.ContainsKey(key))
        //            {
        //                record.MatchingEnergyAnalytics.AddRange(_eaDict[key]);
        //            }
        //        }
        //    }
        //}
        //public void MathWECCWithPlatts()
        //{
        //    if (WECCGISRecords != null)
        //    {
        //        foreach (var record in WECCGISRecords)
        //        {
        //            var key = record.SubstationName;
        //            if (_plattsDict.ContainsKey(key))
        //            {
        //                record.MatchingPlatts.AddRange(_plattsDict[key]);
        //            }
        //        }
        //    }
        //}
        public void MatchRecords()
        {
            _matchedRecordsWECC.Clear();
            _recordsWithoutMatchesWECC.Clear();
            _matchedRecordsPlatts.Clear();
            _recordsWithoutMatchesPlatts.Clear();
            _matchedRecordsEA.Clear();
            _recordsWithoutMatchesEA.Clear();
            if (WECCGISRecords != null)
            {
                foreach (var record in WECCGISRecords)
                {
                    var substationName = record.SubstationName;
                    var busName = record.BusName;
                    var busNumber = record.BusNumber;
                    var noMatch = true;
                    if (_plattsDict != null && _plattsDict.ContainsKey(substationName))
                    {
                        noMatch = false;
                        var matchedPlatss = _plattsDict[substationName];
                        //record.MatchingPlatts.AddRange(matchedPlatss);
                        if (!_matchedRecordsWECC.Contains(record))
                        {
                            _matchedRecordsWECC.Add(record);
                        }
                        foreach (var m in matchedPlatss)
                        {
                            var distance = record.Location.GetDistance(m.Location);
                            record.MatchingPlatts.Add(new Tuple<GISRecord, double>(m, distance));
                            m.MatchingWECC.Add(new Tuple<GISRecord, double>(record, distance));
                            if (!_matchedRecordsPlatts.Contains(m))
                            {
                                _matchedRecordsPlatts.Add(m);
                            }
                        }
                    }
                    if (_eaDict != null && _eaDict.ContainsKey(busName))
                    {
                        noMatch = false;
                        var matchedEA = _eaDict[busName];
                        //record.MatchingEnergyAnalytics.AddRange(matchedEA);
                        if (!_matchedRecordsWECC.Contains(record))
                        {
                            _matchedRecordsWECC.Add(record);
                        }
                        foreach (var m in matchedEA)
                        {
                            var distance = record.Location.GetDistance(m.Location);
                            record.MatchingEnergyAnalytics.Add(new Tuple<GISRecord, double>(m, distance));
                            m.MatchingWECC.Add(new Tuple<GISRecord, double>(record, distance));
                            if (!_matchedRecordsEA.Contains(m))
                            {
                                _matchedRecordsEA.Add(m);
                            }
                        }
                    }
                    else if (_eaDict != null && _eaDict.ContainsKey(substationName))
                    {
                        noMatch = false;
                        var matchedEA = _eaDict[substationName];
                        if (!_matchedRecordsWECC.Contains(record))
                        {
                            _matchedRecordsWECC.Add(record);
                        }
                        foreach (var m in matchedEA)
                        {
                            var distance = record.Location.GetDistance(m.Location);
                            record.MatchingEnergyAnalytics.Add(new Tuple<GISRecord, double>(m, distance));
                            m.MatchingWECC.Add(new Tuple<GISRecord, double>(record, distance));
                            if (!_matchedRecordsEA.Contains(m))
                            {
                                _matchedRecordsEA.Add(m);
                            }
                        }
                    }
                    //else if (_eaBusNumberDict != null && _eaBusNumberDict.ContainsKey(busNumber))
                    //{
                    //    noMatch = false;
                    //    var matchedEA = _eaBusNumberDict[busNumber];
                    //    if (!_matchedRecordsWECC.Contains(record))
                    //    {
                    //        _matchedRecordsWECC.Add(record);
                    //    }
                    //    foreach (var m in matchedEA)
                    //    {
                    //        var distance = record.Location.GetDistance(m.Location);
                    //        record.MatchingEnergyAnalytics.Add(new Tuple<GISRecord, double>(m, distance));
                    //        m.MatchingWECC.Add(new Tuple<GISRecord, double>(record, distance));
                    //        if (!_matchedRecordsEA.Contains(m))
                    //        {
                    //            _matchedRecordsEA.Add(m);
                    //        }
                    //    }
                    //}
                    if (noMatch)
                    {
                        _recordsWithoutMatchesWECC.Add(record);
                    }
                }
            }
            if (ENERGYANAGISRecords != null)
            {
                foreach (var record in ENERGYANAGISRecords)
                {
                    var busName = record.BusName;
                    if (_plattsDict != null && _plattsDict.ContainsKey(busName))
                    {
                        var matchedPlatss = _plattsDict[busName];
                        //record.MatchingPlatts.AddRange(matchedPlatss);
                        if (!_matchedRecordsEA.Contains(record))
                        {
                            _matchedRecordsEA.Add(record);
                        }
                        foreach (var m in matchedPlatss)
                        {
                            var distance = record.Location.GetDistance(m.Location);
                            record.MatchingPlatts.Add(new Tuple<GISRecord, double>(m, distance));
                            m.MatchingEnergyAnalytics.Add(new Tuple<GISRecord, double>(record, distance));
                            if (!_matchedRecordsPlatts.Contains(m))
                            {
                                _matchedRecordsPlatts.Add(m);
                            }
                        }
                    }
                    else if (record.MatchingWECC.Count == 0 && !_recordsWithoutMatchesEA.Contains(record))
                    {
                        _recordsWithoutMatchesEA.Add(record);
                    }
                }
            }
            if (PlattsGISRecords != null)
            {
                foreach (var record in PlattsGISRecords)
                {
                    if (record.MatchingEnergyAnalytics.Count() == 0 && record.MatchingWECC.Count() == 0 && !_recordsWithoutMatchesPlatts.Contains(record))
                    {
                        _recordsWithoutMatchesPlatts.Add(record);
                    }
                }
            }
        }
        private List<GISRecord> _matchedRecordsWECC = new List<GISRecord>();
        private List<GISRecord> _recordsWithoutMatchesWECC = new List<GISRecord>();
        private List<GISRecord> _matchedRecordsPlatts = new List<GISRecord>();
        private List<GISRecord> _recordsWithoutMatchesPlatts = new List<GISRecord>();
        private List<GISRecord> _matchedRecordsEA = new List<GISRecord>();
        private List<GISRecord> _recordsWithoutMatchesEA = new List<GISRecord>();
        public List<GISRecord> GetFilteredResults(bool showWECCLocations, bool showPlattsLocations, bool showEnergyAnalyticsLocations, bool showMatchedLocation, DistanceFilterEnum value)
        {
            int distanceFilter = 0;
            if (int.TryParse(value.ToString(), out distanceFilter))
            {

            }
            
            return new List<GISRecord>();
        }
        public void ClearEARecords()
        {
            _recordsWithoutMatchesEA.Clear();
            _matchedRecordsEA.Clear();
            foreach (var item in _matchedRecordsPlatts)
            {
                item.MatchingEnergyAnalytics.Clear();
            }
            foreach (var item in _matchedRecordsWECC)
            {
                item.MatchingEnergyAnalytics.Clear();
            }
        }
        public void ClearPlattsRecords()
        {
            _recordsWithoutMatchesPlatts.Clear();
            _matchedRecordsPlatts.Clear();
            foreach (var item in _matchedRecordsEA)
            {
                item.MatchingPlatts.Clear();
            }
            foreach (var item in _matchedRecordsWECC)
            {
                item.MatchingPlatts.Clear();
            }
        }
        public void ClearWECCRecords()
        {
            _recordsWithoutMatchesWECC.Clear();
            _matchedRecordsWECC.Clear();
            foreach (var item in _matchedRecordsEA)
            {
                item.MatchingWECC.Clear();
            }
            foreach (var item in _matchedRecordsPlatts)
            {
                item.MatchingWECC.Clear();
            }
        }

        public void GetUnmatchedResults(MapViewModel mapVM)
        {
            var weccMarkers = _recordsWithoutMatchesWECC.Select(x => x.Marker).ToList();
            var plattsMarkers = _recordsWithoutMatchesPlatts.Select(x => x.Marker).ToList();
            var eaMarkers = _recordsWithoutMatchesEA.Select(x => x.Marker).ToList();
            mapVM.SetFilteredMarkers(weccMarkers, plattsMarkers, eaMarkers);
        }

        public void GetMatchedResults(MapViewModel mapVM, DistanceFilterEnum selectedDistanceFilter, CompareOperatorEnum selectedCompareSign)
        {
            int filter;
            var descriptionAttri = selectedDistanceFilter.GetType().GetField(selectedDistanceFilter.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault() as DescriptionAttribute;
            var description = descriptionAttri.Description;
            if (int.TryParse(description, out filter))
            {
                if (selectedCompareSign == CompareOperatorEnum.GreaterThan)
                {
                    _getMarkerGreaterThanFilter(mapVM, filter);
                }
                if (selectedCompareSign == CompareOperatorEnum.LessOrEqual)
                {
                    _getMarkerLessOrEqThanFilter(mapVM, filter);
                }
            }
        }

        private void _getMarkerLessOrEqThanFilter(MapViewModel mapVM, int filter)
        {
            var weccMarkers = new List<GMapMarker>();
            var plattsMarkers = new List<GMapMarker>();
            var eaMarkers = new List<GMapMarker>();
            //var addedEARecords = new List<GISRecord>();
            foreach (var item in _matchedRecordsWECC)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            weccMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        eaMarkers.Add(pair.Item1.Marker);
                    }
                }
                foreach (var pair in item.MatchingPlatts)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            weccMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        plattsMarkers.Add(pair.Item1.Marker);
                    }
                }
            }
            foreach (var item in _matchedRecordsPlatts)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            plattsMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        eaMarkers.Add(pair.Item1.Marker);
                    }
                }
            }
            mapVM.SetFilteredMarkers(weccMarkers.Distinct().ToList(), plattsMarkers.Distinct().ToList(), eaMarkers.Distinct().ToList());
        }

        private void _getMarkerGreaterThanFilter(MapViewModel mapVM, int filter)
        {
            var weccMarkers = new List<GMapMarker>();
            var plattsMarkers = new List<GMapMarker>();
            var eaMarkers = new List<GMapMarker>();
            //var addedEARecords = new List<GISRecord>();
            foreach (var item in _matchedRecordsWECC)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            weccMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        eaMarkers.Add(pair.Item1.Marker);
                    }
                }
                foreach (var pair in item.MatchingPlatts)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            weccMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        plattsMarkers.Add(pair.Item1.Marker);
                    }
                }
            }
            foreach (var item in _matchedRecordsPlatts)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            plattsMarkers.Add(item.Marker);
                            itemAdded = true;
                        }
                        eaMarkers.Add(pair.Item1.Marker);
                    }
                }
            }
            mapVM.SetFilteredMarkers(weccMarkers.Distinct().ToList(), plattsMarkers.Distinct().ToList(), eaMarkers.Distinct().ToList());
        }

        public Dictionary<string, List<GISRecord>> GetUnmatchedGISRecords()
        {
            var result = new Dictionary<string, List<GISRecord>>();
            if (_recordsWithoutMatchesWECC.Count() > 0)
            {
                result["WECC"] = _recordsWithoutMatchesWECC;
            }
            if (_recordsWithoutMatchesPlatts.Count() > 0)
            {
                result["Platts"] = _recordsWithoutMatchesPlatts;
            }
            if (_recordsWithoutMatchesEA.Count() > 0)
            {
                result["EnergyAnalytics"] = _recordsWithoutMatchesEA;
            }
            return result;
        }

        public Dictionary<string, List<GISRecord>> GetMatchedRecords(CompareOperatorEnum selectedCompareSign, string description)
        {
            int filter;
            Dictionary<string, List<GISRecord>> result = null;
            if (int.TryParse(description, out filter))
            {
                if (selectedCompareSign == CompareOperatorEnum.GreaterThan)
                {
                    result = _getRecordsGreaterThanFilter(filter);
                }
                if (selectedCompareSign == CompareOperatorEnum.LessOrEqual)
                {
                    result = _getRecordsLessOrEqThanFilter(filter);
                }
            }
            return result;
        }

        private Dictionary<string, List<GISRecord>> _getRecordsLessOrEqThanFilter(int filter)
        {
            var weccGISRecord = new List<GISRecord>();
            var plattsGISRecord = new List<GISRecord>();
            var eaGISRecord = new List<GISRecord>();
            //var addedEARecords = new List<GISRecord>();
            foreach (var item in _matchedRecordsWECC)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            weccGISRecord.Add(item);
                            itemAdded = true;
                        }
                        eaGISRecord.Add(pair.Item1);
                    }
                }
                foreach (var pair in item.MatchingPlatts)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            weccGISRecord.Add(item);
                            itemAdded = true;
                        }
                        plattsGISRecord.Add(pair.Item1);
                    }
                }
            }
            foreach (var item in _matchedRecordsPlatts)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 <= filter)
                    {
                        if (!itemAdded)
                        {
                            plattsGISRecord.Add(item);
                            itemAdded = true;
                        }
                        eaGISRecord.Add(pair.Item1);
                    }
                }
            }
            var result = new Dictionary<string, List<GISRecord>>();
            if (weccGISRecord.Count() > 0)
            {
                result["WECC"] = weccGISRecord.Distinct().ToList();
            }
            if (plattsGISRecord.Count() > 0)
            {
                result["Platts"] = plattsGISRecord.Distinct().ToList();
            }
            if (eaGISRecord.Count() > 0)
            {
                result["EnergyAnalytics"] = eaGISRecord.Distinct().ToList();
            }
            return result;
        }

        private Dictionary<string, List<GISRecord>> _getRecordsGreaterThanFilter(int filter)
        {
            var weccGISRecord = new List<GISRecord>();
            var plattsGISRecord = new List<GISRecord>();
            var eaGISRecord = new List<GISRecord>();
            foreach (var item in _matchedRecordsWECC)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            weccGISRecord.Add(item);
                            itemAdded = true;
                        }
                        eaGISRecord.Add(pair.Item1);
                    }
                }
                foreach (var pair in item.MatchingPlatts)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            weccGISRecord.Add(item);
                            itemAdded = true;
                        }
                        plattsGISRecord.Add(pair.Item1);
                    }
                }
            }
            foreach (var item in _matchedRecordsPlatts)
            {
                bool itemAdded = false;
                foreach (var pair in item.MatchingEnergyAnalytics)
                {
                    if (pair.Item2 > filter)
                    {
                        if (!itemAdded)
                        {
                            plattsGISRecord.Add(item);
                            itemAdded = true;
                        }
                        eaGISRecord.Add(pair.Item1);
                    }
                }
            }
            var result = new Dictionary<string, List<GISRecord>>();
            if (weccGISRecord.Count() > 0)
            {
                result["WECC"] = weccGISRecord.Distinct().ToList();
            }
            if (plattsGISRecord.Count() > 0)
            {
                result["Platts"] = plattsGISRecord.Distinct().ToList();
            }
            if (eaGISRecord.Count() > 0)
            {
                result["EnergyAnalytics"] = eaGISRecord.Distinct().ToList();
            }
            return result;
        }
    }
}
