using ExcelReader;
using GISData.Model;
using MapService.Models;
using MapService.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
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
            SelectedDistanceFilter = DistanceFilterEnum.Infinity;
            _hasNewRecords = false;
            CompareGISRecords = new RelayCommand(_compareGISRecords);
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
                    _recordComparer.WECCGISRecords = WECCGISRecords;
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
                    _recordComparer.PlattsGISRecords = PlattsGISRecords;
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
                    _recordComparer.ENERGYANAGISRecords = ENERGYANAGISRecords;
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
                _selectedDistanceFilter = value;
                OnPropertyChanged();
                if ((ShowMatchedLocation && SelectedDistanceFilter != DistanceFilterEnum.Infinity) || !ShowMatchedLocation && SelectedDistanceFilter != DistanceFilterEnum.Zero)
                {
                    var filteredResults = _recordComparer.GetFilteredResults(MapVM.ShowWECCLocations, MapVM.ShowPlattsLocations, MapVM.ShowEnergyAnalyticsLocations, ShowMatchedLocation, value);
                }
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

    }
}
