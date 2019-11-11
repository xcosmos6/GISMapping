using ExcelReader;
using MapService.ViewModels;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities;

namespace NAERMMap.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            MapVM = new MapViewModel();
            _xcelReader = new XcelReader();
            BrowseGISFiles = new RelayCommand(_browseGISFiles);
            _gisFilePath = NAERMMap.Properties.Settings.Default.GISFilePath;
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
        private XcelReader _xcelReader;
        private string _gisFilePath;
        public string GISFilePath
        {
            get { return _gisFilePath; }
            set
            {
                _gisFilePath = value;
                Properties.Settings.Default.GISFilePath = value;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        private string _gisFileFullPath;
        public string GISFileFullPath
        {
            get { return _gisFileFullPath; }
            set
            {
                _gisFileFullPath = value;
                GISFilePath = Path.GetDirectoryName(value);
                var gis = _xcelReader.Read(value);
                MapVM.PlotGIS(gis);
                OnPropertyChanged();
            }
        }
        public ICommand BrowseGISFiles { get; set; }
        private void _browseGISFiles(object obj)
        {
            using (var fbd = new CommonOpenFileDialog())
            {
                fbd.InitialDirectory = GISFilePath;
                fbd.IsFolderPicker = false;
                fbd.AddToMostRecentlyUsedList = true;
                fbd.AllowNonFileSystemItems = false;
                fbd.DefaultDirectory = GISFilePath;
                fbd.EnsureFileExists = true;
                fbd.EnsurePathExists = true;
                fbd.EnsureReadOnly = false;
                fbd.EnsureValidNames = true;
                fbd.Multiselect = false;
                fbd.ShowPlacesList = true;
                fbd.RestoreDirectory = true;
                CommonFileDialogResult result = fbd.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    GISFileFullPath = fbd.FileName;
                }
            }
        }
    }
}
