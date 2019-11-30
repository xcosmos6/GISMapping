using MapService.Models;
using MapService.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Utilities;

namespace NAERMMap.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            MapVM = new MapViewModel();
            //_xcelReader = new XcelReader();
            _csvReader = new CSVReader();
            WECCBrowseGISFiles = new RelayCommand(_weccbrowseGISFiles);
            _weccgisFilePath = NAERMMap.Properties.Settings.Default.WECCGISFilePath;
            PlattsBrowseGISFiles = new RelayCommand(_plattsbrowseGISFiles);
            _plattsgisFilePath = NAERMMap.Properties.Settings.Default.PlattsGISFilePath;
            ENERGYANABrowseGISFiles = new RelayCommand(_energyanabrowseGISFiles);
            _energyanagisFilePath = NAERMMap.Properties.Settings.Default.ENERGYANAGISFilePath;
            GISRecords = new List<GISRecord>();
            WECCGISRecords = new List<GISRecord>();
            PlattsGISRecords = new List<GISRecord>();
            ENERGYANAGISRecords = new List<GISRecord>();
            _recordComparer = new GISRecordComparer();
            _showMatchedLocation = true;
            //DistanceFilters = new List<int>() { 0, 1, 5, 10, 50, 100, 500, 1000, 5000, 10000, 100000, int.MaxValue};
            SelectedDistanceFilter = DistanceFilterEnum.Zero;
            _hasNewRecords = false;
            CompareGISRecords = new RelayCommand(_compareGISRecords);
            MarkerFilterChoices = new List<string>() { "Show All GIS Records", "Show Records Have NO Match", "Show Records Have Matchs" };
            _selectedChoice = "Show All GIS Records";
            MapVM.NewMarkersCreated += MapVM_NewMarkersCreated;
            ExportFilteredResults = new RelayCommand(_exportFilteredResults);
        }
        private MapViewModel _mapVM;
        public MapViewModel MapVM
        {
            get { return _mapVM; }
            set
            {
                _mapVM = value;
                OnPropertyChanged();
            }
        }
        //private XcelReader _xcelReader;
        private CSVReader _csvReader;
        private GISRecordComparer _recordComparer;
        public List<GISRecord> GISRecords { get; set; }
        public List<GISRecord> WECCGISRecords { get; set; }
        public List<GISRecord> PlattsGISRecords { get; set; }
        public List<GISRecord> ENERGYANAGISRecords { get; set; }
        //private void _updateGISRecords()
        //{
        //    GISRecords.Clear();
        //    GISRecords.AddRange(ENERGYANAGISRecords);
        //    GISRecords.AddRange(PlattsGISRecords);
        //    GISRecords.AddRange(WECCGISRecords);
        //    //MapVM.PlotGIS(GISRecords);
        //}

        //to read in WECC GIS
        private string _weccgisFilePath;
        public string WECCGISFilePath
        {
            get { return _weccgisFilePath; }
            set
            {
                _weccgisFilePath = value;
                Properties.Settings.Default.WECCGISFilePath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        private string _weccgisFileFullPath;
        public string WECCGISFileFullPath
        {
            get { return _weccgisFileFullPath; }
            set
            {
                if (_weccgisFileFullPath != value)
                {
                    _weccgisFileFullPath = value;
                    HasNewRecords = true;
                    OnPropertyChanged();
                    MapVM.ClearWECCMarkers();
                    _recordComparer.ClearWECCRecords();
                    if (File.Exists(_weccgisFileFullPath))
                    {
                        try
                        {
                            WECCGISFilePath = Path.GetDirectoryName(value);
                            WECCGISRecords = _csvReader.Read(WECCGISFileFullPath, "WECC");
                            MapVM.PlotGIS(WECCGISRecords);
                            //_recordComparer.WECCGISRecords = WECCGISRecords;
                            ////_updateGISRecords();
                            //MapVM.ClearWECCMarkers();
                            //MapVM.PlotGIS(WECCGISRecords);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        WECCGISRecords = new List<GISRecord>();
                    }
                    try
                    {
                        _recordComparer.WECCGISRecords = WECCGISRecords;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading WECC GIS file: " + value + " . " + ex.Message);
                    }
                }
            }
        }
        public ICommand WECCBrowseGISFiles { get; set; }
        private void _weccbrowseGISFiles(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = WECCGISFilePath;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = WECCGISFilePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select WECC CSV data file.";
                fbd.Filters.Add(new CommonFileDialogFilter("CSV files (*.csv)", "*.csv"));
                fbd.Filters.Add(new CommonFileDialogFilter("All files (*.*)", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    WECCGISFileFullPath = fbd.FileName;
                }
            }
        }
        
        //to read in platts GIS
        private string _plattsgisFilePath;
        public string PlattsGISFilePath
        {
            get { return _plattsgisFilePath; }
            set
            {
                _plattsgisFilePath = value;
                Properties.Settings.Default.PlattsGISFilePath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        private string _plattsgisFileFullPath;
        public string PlattsGISFileFullPath
        {
            get { return _plattsgisFileFullPath; }
            set
            {
                if (_plattsgisFileFullPath != value)
                {
                    _plattsgisFileFullPath = value;
                    HasNewRecords = true;
                    OnPropertyChanged();
                    MapVM.ClearPlattsMarkers();
                    _recordComparer.ClearPlattsRecords();
                    if (File.Exists(_plattsgisFileFullPath))
                    {
                        try
                        {
                            PlattsGISFilePath = Path.GetDirectoryName(value);
                            PlattsGISRecords = _csvReader.Read(PlattsGISFileFullPath, "Platts");
                            //_updateGISRecords();
                            MapVM.PlotGIS(PlattsGISRecords);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        PlattsGISRecords = new List<GISRecord>();
                    }
                    try
                    {
                        _recordComparer.PlattsGISRecords = PlattsGISRecords;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error reading Platts GIS csv file: " + value + " . " + ex.Message);
                    }
                }
            }
        }
        public ICommand PlattsBrowseGISFiles { get; set; }
        private void _plattsbrowseGISFiles(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = PlattsGISFilePath;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory =PlattsGISFilePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Platts CSV data file.";
                fbd.Filters.Add(new CommonFileDialogFilter("CSV files (*.csv)", "*.csv"));
                fbd.Filters.Add(new CommonFileDialogFilter("All files (*.*)", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    PlattsGISFileFullPath = fbd.FileName;
                }
            }
        }

        //to read in ENERGYANA GIS
        private string _energyanagisFilePath;
        public string ENERGYANAGISFilePath
        {
            get { return _energyanagisFilePath; }
            set
            {
                _energyanagisFilePath = value;
                Properties.Settings.Default.ENERGYANAGISFilePath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        private string _energyanagisFileFullPath;
        public string ENERGYANAGISFileFullPath
        {
            get { return _energyanagisFileFullPath; }
            set
            {
                if (_energyanagisFileFullPath != value)
                {
                    _energyanagisFileFullPath = value;
                    HasNewRecords = true;
                    OnPropertyChanged();
                    MapVM.ClearEnergyAnalyticsMarkers();
                    _recordComparer.ClearEARecords();
                    if (File.Exists(_energyanagisFileFullPath))
                    {
                        try
                        {
                            ENERGYANAGISFilePath = Path.GetDirectoryName(value);
                            ENERGYANAGISRecords = _csvReader.Read(ENERGYANAGISFileFullPath, "ENERGYANA");
                            //_updateGISRecords();
                            MapVM.PlotGIS(ENERGYANAGISRecords);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        ENERGYANAGISRecords = new List<GISRecord>();
                    }
                    try
                    {
                        _recordComparer.ENERGYANAGISRecords = ENERGYANAGISRecords;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro reading energy analytics GIS csv file: " + value + " . " + ex.Message) ;
                    }
                }
            }
        }
        public ICommand ENERGYANABrowseGISFiles { get; set; }
        private void _energyanabrowseGISFiles(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = ENERGYANAGISFilePath;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = ENERGYANAGISFilePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                fbd.Title = "Please Select Energy Analytics CSV data file.";
                fbd.Filters.Add(new CommonFileDialogFilter("CSV files (*.csv)", "*.csv"));
                fbd.Filters.Add(new CommonFileDialogFilter("All files (*.*)", "*.*"));
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    ENERGYANAGISFileFullPath = fbd.FileName;
                }
            }
        }
        private bool _showMatchedLocation;
        public bool ShowMatchedLocation 
        {
            get { return _showMatchedLocation; }
            set { _showMatchedLocation = value;
                OnPropertyChanged();
            }
        }
        //public List<int> DistanceFilters { get; set; }
        private DistanceFilterEnum _selectedDistanceFilter;
        public DistanceFilterEnum SelectedDistanceFilter 
        {
            get { return _selectedDistanceFilter; }
            set
            {
                if (_selectedDistanceFilter != value)
                {
                    _selectedDistanceFilter = value;
                    OnPropertyChanged();
                    _recordComparer.GetMatchedResults(MapVM, _selectedDistanceFilter, _selectedCompareSign);
                    MapVM.UpdateMarkersOnMap();
                }
                //if ((ShowMatchedLocation && SelectedDistanceFilter != DistanceFilterEnum.Infinity) || !ShowMatchedLocation && SelectedDistanceFilter != DistanceFilterEnum.Zero)
                //{
                //    var filteredResults = _recordComparer.GetFilteredResults(MapVM.ShowWECCLocations, MapVM.ShowPlattsLocations, MapVM.ShowEnergyAnalyticsLocations, ShowMatchedLocation, value);
                //}
            }
        }
        private bool _hasNewRecords;
        public bool HasNewRecords 
        {
            get { return _hasNewRecords; }
            set
            {
                _hasNewRecords = value;
                OnPropertyChanged();
            }
        }
        public ICommand CompareGISRecords { get; set; }
        private void _compareGISRecords(object obj)
        {
            HasNewRecords = false;
            _recordComparer.MatchRecords();
        }
        public List<string> MarkerFilterChoices { get; set; }
        private string _selectedChoice;
        public string SelectedChoice 
        {
            get { return _selectedChoice; }
            set
            {
                if (_selectedChoice != value)
                {
                    _selectedChoice = value;
                    OnPropertyChanged();
                    _updateMarkersOnMap();
                }
            }
        }

        private void _updateMarkersOnMap()
        {
            switch (_selectedChoice)
            {
                case "Show All GIS Records":
                    MapVM.SetFilteredMarkers();
                    break;
                case "Show Records Have NO Match":
                    _recordComparer.GetUnmatchedResults(MapVM);
                    break;
                case "Show Records Have Matchs":
                    _recordComparer.GetMatchedResults(MapVM, _selectedDistanceFilter, _selectedCompareSign);
                    break;
                default:
                    break;
            }
            MapVM.UpdateMarkersOnMap();
        }

        private void MapVM_NewMarkersCreated()
        {
            _updateMarkersOnMap();
        }
        private CompareOperatorEnum _selectedCompareSign;
        public CompareOperatorEnum SelectedCompareSign 
        {
            get { return _selectedCompareSign; }
            set
            {
                if (_selectedCompareSign != value)
                {
                    _selectedCompareSign = value;
                    OnPropertyChanged();
                    _recordComparer.GetMatchedResults(MapVM, _selectedDistanceFilter, _selectedCompareSign);
                    MapVM.UpdateMarkersOnMap();
                }
            }
        }
        public ICommand ExportFilteredResults { get; set; }
        private void _exportFilteredResults(object obj)
        {
            switch (_selectedChoice)
            {
                case "Show All GIS Records":
                    break;
                case "Show Records Have NO Match":
                    //write unmatched records to csv files
                    break;
                case "Show Records Have Matchs":
                    // write matched records to csv files according to SelectedCompareSign and SelectedDistanceFilter
                    break;
                default:
                    break;
            }
        }

    }
}
