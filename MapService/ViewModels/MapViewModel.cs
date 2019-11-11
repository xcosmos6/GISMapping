﻿using ExcelReader;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Utilities;

namespace MapService.ViewModels
{
    public class MapViewModel : ViewModelBase
    {
        public MapViewModel()
        {
            SetUpGMap();
            //Annotations = new List<PointAndInfo>();
            ChangeZoom = new RelayCommand(_changeMapZoom);
            ZoomIn = new RelayCommand(_mapZoomIn);
            ZoomOut = new RelayCommand(_mapZoomOut);
        }
        public void SetUpGMap()
        {
            try
            {
                Gmap = new GMapControl();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Gmap.MaxZoom = MaxZoom;
            Gmap.MinZoom = MinZoom;
            Gmap.Zoom = 5;
            Gmap.CenterPosition = new PointLatLng(37.0902, -95.7129);
            Gmap.ShowCenter = false;
            Gmap.MapProvider = GoogleSatelliteMapProvider.Instance; //satellite view
            Gmap.Manager.Mode = AccessMode.ServerAndCache;
            var cwd = Directory.GetCurrentDirectory();
            var oneLevelUp = cwd.Substring(0, cwd.LastIndexOf(@"\"));
            Gmap.CacheLocation = oneLevelUp + @"\MapCache";
            Console.WriteLine("The Gmap.CacheLocation is {0}", Gmap.CacheLocation);
            Gmap.MouseMove += GMap_MouseMove;
            //Gmap.MouseLeftButtonDown += GMap_MouseLeftButtonDown;
        }

        public void PlotGIS(List<GISRecord> gis)
        {
            Gmap.Markers.Clear();
            var gisBySource = gis.GroupBy(x => x.Source);
            foreach (var item in gisBySource)
            {
                var source = item.Key;
                var gisList = item.ToList();
                foreach (var g in gisList)
                {
                    //var point = Gmap.FromLatLngToLocal(new PointLatLng(g.Latitude, g.Longitude));
                    var newMarker = new GMapMarker(new PointLatLng(g.Latitude, g.Longitude));
                    {
                        //newMarker.Offset = new System.Windows.Point(-12.5, -25);
                        if (source == "WECC")
                        {
                            newMarker.Shape = new Image
                            {
                                Width = 25,
                                Height = 25,
                                Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerGreen.png", UriKind.Relative)),
                                ToolTip = g.Description
                            };
                        }
                        if (source == "Platts")
                        {
                            newMarker.Shape = new Image
                            {
                                Width = 25,
                                Height = 25,
                                Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerYellow.png", UriKind.Relative)),
                                ToolTip = g.Description
                            };
                        }
                        if (source == "ENERGYANA")
                        {
                            newMarker.Shape = new Image
                            {
                                Width = 25,
                                Height = 25,
                                Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerRed.png", UriKind.Relative)),
                                ToolTip = g.Description
                            };
                        }
                        //newMarker.Tag = g.Description;
                    }
                    Gmap.Markers.Add(newMarker);
                }
            }
        }

        private GMapControl _gMap;
        public GMapControl Gmap
        {
            get { return _gMap; }
            set
            {
                _gMap = value;
                OnPropertyChanged();
            }
        }
        public int MaxZoom { get; set; } = 20;
        public int MinZoom { get; set; } = 0;
        public ICommand ChangeZoom { get; set; }
        private void _changeMapZoom(object obj)
        {
            Gmap.Zoom = Gmap.Zoom;
        }
        public ICommand ZoomIn { get; set; }
        private void _mapZoomIn(object obj)
        {
            Gmap.Zoom = Gmap.Zoom + 1;
        }
        public ICommand ZoomOut { get; set; }
        private void _mapZoomOut(object obj)
        {
            Gmap.Zoom = Gmap.Zoom - 1;
        }
        private double _currentLat;
        public double CurrentLat
        {
            get { return _currentLat; }
            set
            {
                _currentLat = value;
                OnPropertyChanged();
            }
        }
        private double _currentLng;
        public double CurrentLng
        {
            get { return _currentLng; }
            set
            {
                _currentLng = value;
                OnPropertyChanged();
            }
        }
        private void GMap_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var gctl = sender as GMapControl;
            var ps = e.GetPosition(gctl);
            CurrentLng = gctl.FromLocalToLatLng((int)ps.X, (int)ps.Y).Lng;
            CurrentLat = gctl.FromLocalToLatLng((int)ps.X, (int)ps.Y).Lat;
        }
    }
}
