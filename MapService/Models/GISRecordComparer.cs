using GISData.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                }
            }
        }
        private Dictionary<string, List<GISRecord>> _eaDict;
        private Dictionary<string, List<GISRecord>> _plattsDict;
        public void MathWECCWithEnergyAnalytics()
        {
            if (WECCGISRecords != null)
            {
                foreach (var record in WECCGISRecords)
                {
                    var key = record.SubstationName;
                    if (_eaDict.ContainsKey(key))
                    {
                        record.MatchingEnergyAnalytics.AddRange(_eaDict[key]);
                    }
                }
            }
        }
        public void MathWECCWithPlatts()
        {
            if (WECCGISRecords != null)
            {
                foreach (var record in WECCGISRecords)
                {
                    var key = record.SubstationName;
                    if (_plattsDict.ContainsKey(key))
                    {
                        record.MatchingPlatts.AddRange(_plattsDict[key]);
                    }
                }
            }
        }
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
                    var noMatch = true;
                    if (_plattsDict != null && _plattsDict.ContainsKey(substationName))
                    {
                        noMatch = false;
                        var matchedPlatss = _plattsDict[substationName];
                        record.MatchingPlatts.AddRange(matchedPlatss);
                        if (!_matchedRecordsWECC.Contains(record))
                        {
                            _matchedRecordsWECC.Add(record);
                        }
                        foreach (var m in matchedPlatss)
                        {
                            m.MatchingWECC.Add(record);
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
                        record.MatchingEnergyAnalytics.AddRange(matchedEA);
                        if (!_matchedRecordsWECC.Contains(record))
                        {
                            _matchedRecordsWECC.Add(record);
                        }
                        foreach (var m in matchedEA)
                        {
                            m.MatchingWECC.Add(record);
                            if (!_matchedRecordsEA.Contains(m))
                            {
                                _matchedRecordsEA.Add(m);
                            }
                        }
                    }
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
                        record.MatchingPlatts.AddRange(matchedPlatss);
                        if (!_matchedRecordsEA.Contains(record))
                        {
                            _matchedRecordsEA.Add(record);
                        }
                        foreach (var m in matchedPlatss)
                        {
                            m.MatchingEnergyAnalytics.Add(record);
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
            //MathWECCWithEnergyAnalytics();
            //MathWECCWithPlatts();
            //MatchRecords();
            return new List<GISRecord>();
        }
    }
}
