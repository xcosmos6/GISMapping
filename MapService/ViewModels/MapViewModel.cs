using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using MapService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            FindLocation = new RelayCommand(_findLocation);
            _measureLocation1 = null;
            _measureLocation2 = null;
            ShowWECCLocations = false;
            ShowPlattsLocations = false;
            ShowEnergyAnalyticsLocations = false;
            _filteredEnergyAnalyticsMarkerList = new List<GMapMarker>();
            _filteredWECCMarkerList = new List<GMapMarker>();
            _filteredPlattsMarkerList = new List<GMapMarker>();
            _allWECCMarkerList = new List<GMapMarker>();
            _allPlattsMarkerList = new List<GMapMarker>();
            _allEnergyAnalyticsMarkerList = new List<GMapMarker>();
            _matchedLocations = new List<GMapMarker>();
            _locationToBeFoundX = -95.7129;
            _locationToBeFoundY = 37.0902;
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
            Gmap.IgnoreMarkerOnMouseWheel = true;
            Gmap.AllowDrop = true;
            //Gmap.ContextMenu = new ContextMenu();
            //Gmap.DragButton = MouseButton.Left;
            Gmap.MouseRightButtonUp += _showContextMenu;
        }
        private LatLngPoint _measureLocation1;
        public LatLngPoint MeasureLocation1
        {
            get { return _measureLocation1; }
            set
            {
                _measureLocation1 = value;
                OnPropertyChanged();
            }
        }
        private LatLngPoint _measureLocation2;
        public LatLngPoint MeasureLocation2
        {
            get { return _measureLocation2; }
            set
            {
                _measureLocation2 = value;
                OnPropertyChanged();
            }
        }
        private double _measuredDistance;
        public double MeasuredDistance
        {
            get { return _measuredDistance; }
            set
            {
                _measuredDistance = value;
                OnPropertyChanged();
                if (value != 0 && MeasureLocation1 != null && MeasureLocation2 != null)
                {
                    if (_measuredDistanceLine != null)
                    {
                        Gmap.Markers.Remove(_measuredDistanceLine);
                    }
                    _drawMeasuredLine();
                }
            }
        }
        private GMapRoute _measuredDistanceLine;
        private void _drawMeasuredLine()
        {
            var newLine = new List<PointLatLng>();
            newLine.Add(MeasureLocation1.GetPointLatLng());
            newLine.Add(MeasureLocation2.GetPointLatLng());
            var newRoute = new GMapRoute(newLine);
            newRoute.Shape = new System.Windows.Shapes.Path() { StrokeThickness = 4, ToolTip = MeasuredDistance, Stroke = Brushes.Blue };
            newRoute.Tag = MeasuredDistance;
            newRoute.ZIndex = 0;
            Gmap.Markers.Add(newRoute);
            _measuredDistanceLine = newRoute;
            newRoute.Shape.MouseRightButtonUp += _removeMeasuredDistanceLine;
        }
        private void _removeMeasuredDistanceLine(object sender, MouseButtonEventArgs e)
        {
            Gmap.Markers.Remove(_measuredDistanceLine);
            MeasureLocation1 = null;
            MeasureLocation2 = null;
            MeasuredDistance = 0;
            _measuredDistanceLine = null;
        }
        private void _showContextMenu(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                var currentloc = e.GetPosition(Gmap);
                var position = Gmap.FromLocalToLatLng((int)currentloc.X, (int)currentloc.Y);
                var cm = new ContextMenu();
                //cm.Items.Add(new MenuItem("Measure Distance From", new ErrorEventHandler(_addFromLocation)));
                var m1 = new MenuItem();
                var m2 = new MenuItem();
                m1.Header = "Measure Distance Location 1";
                m1.Click += new RoutedEventHandler((s,ee)=> _add1stLocation(s,ee,new LatLngPoint(position)));
                cm.Items.Add(m1);
                if (MeasureLocation1 != null)
                {
                    m2.Header = "Measure Distance Location 2";
                    m2.Click += new RoutedEventHandler((s, ee) => _add2ndLocation(s, ee, new LatLngPoint(position)));
                    cm.Items.Add(m2);
                }
                cm.IsOpen = true;
            }
        }
        private void _add2ndLocation(object sender, RoutedEventArgs e, LatLngPoint lc)
        {
            MeasureLocation2 = lc;
            if (_measureLocation1 != null)
            {
                MeasuredDistance = MeasureLocation2.GetDistance(MeasureLocation1);
            }
        }
        private void _add1stLocation(object sender, RoutedEventArgs e, LatLngPoint lc)
        {
            MeasureLocation1 = lc;
            if (_measureLocation2 != null)
            {
                MeasuredDistance = MeasureLocation1.GetDistance(MeasureLocation2);
            }
        }
        public void PlotGIS(List<GISRecord> gis)
        {
            //Gmap.Markers.Clear();
            foreach (var item in gis)
            {
                var newMarker = new GMapMarker(new PointLatLng(item.Location.Lat, item.Location.Lng));
                if (item.Source == "WECC")
                {
                    newMarker.Shape = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Stroke = Brushes.LightGreen,
                        Fill = Brushes.LightGreen,
                        ToolTip = item.Description,
                        Tag = item
                    };
                    newMarker.Shape.MouseLeftButtonUp += WECCMarker_MouseLeftButtonUp;
                    newMarker.Shape.MouseRightButtonUp += Shape_MouseRightButtonUp;
                    _allWECCMarkerList.Add(newMarker);
                    if (ShowWECCLocations)
                    {
                        Gmap.Markers.Add(newMarker);
                    }
                }
                if (item.Source == "Platts")
                {
                    var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffff99"));
                    newMarker.Shape = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Stroke = brush,
                        Fill = brush,
                        ToolTip = item.Description,
                        Tag = item
                    };
                    newMarker.Shape.MouseLeftButtonUp += PlattsMarker_MouseLeftButtonUp;
                    newMarker.Shape.MouseRightButtonUp += Shape_MouseRightButtonUp;
                    _allPlattsMarkerList.Add(newMarker);
                    if (ShowPlattsLocations)
                    {
                        Gmap.Markers.Add(newMarker);
                    }
                }
                if (item.Source == "ENERGYANA")
                {
                    newMarker.Shape = new Ellipse
                    {
                        Width = 15,
                        Height = 15,
                        Stroke = Brushes.LightPink,
                        Fill = Brushes.LightPink,
                        ToolTip = item.Description,
                        Tag = item
                    };
                    newMarker.Shape.MouseLeftButtonUp += ENERGYANAMarker_MouseLeftButtonUp;
                    newMarker.Shape.MouseRightButtonUp += Shape_MouseRightButtonUp;
                    _allEnergyAnalyticsMarkerList.Add(newMarker);
                    if (ShowEnergyAnalyticsLocations)
                    {
                        Gmap.Markers.Add(newMarker);
                    }
                }
                newMarker.Tag = item;
                newMarker.ZIndex = 0;
                newMarker.Offset = new Point(-7.5, -7.5);
                item.Marker = newMarker;
            }
            if (gis.Count() > 0)
            {
                OnNewMarkersCreated();
            }
        }
        private List<GMapMarker> _matchedLocations;
        private void Shape_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (_matchedLocations != null)
            {
                foreach (var mkr in _matchedLocations)
                {
                    Gmap.Markers.Remove(mkr);
                }
                _matchedLocations.Clear();
            }
            var s = sender as Ellipse;
            var record = s.Tag as GISRecord;
            foreach (var item in record.MatchingEnergyAnalytics)
            {
                var lc = new PointLatLng(item.Item1.Location.Lat, item.Item1.Location.Lng);
                var newMarker = new GMapMarker(lc);
                newMarker.Shape = new Image
                {
                    Width = 30,
                    Height = 30,
                    Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerRed.png", UriKind.Relative)),
                    ToolTip = item.Item2
                };
                newMarker.Tag = "Matched Location";
                newMarker.Offset = new Point(-15, -30);
                newMarker.ZIndex = 1000;
                _matchedLocations.Add(newMarker);
                Gmap.Markers.Add(newMarker);
                newMarker.Shape.MouseRightButtonUp += _removeMatchedLocation;
            }
            foreach (var item in record.MatchingPlatts)
            {
                var lc = new PointLatLng(item.Item1.Location.Lat, item.Item1.Location.Lng);
                var newMarker = new GMapMarker(lc);
                newMarker.Shape = new Image
                {
                    Width = 30,
                    Height = 30,
                    Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerYellow.png", UriKind.Relative)),
                    ToolTip = item.Item2
                };
                newMarker.Tag = "Matched Location";
                newMarker.Offset = new Point(-15, -30);
                newMarker.ZIndex = 1000;
                _matchedLocations.Add(newMarker);
                Gmap.Markers.Add(newMarker);
                newMarker.Shape.MouseRightButtonUp += _removeMatchedLocation;
            }
            foreach (var item in record.MatchingWECC)
            {
                var lc = new PointLatLng(item.Item1.Location.Lat, item.Item1.Location.Lng);
                var newMarker = new GMapMarker(lc);
                newMarker.Shape = new Image
                {
                    Width = 30,
                    Height = 30,
                    Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerGreen.png", UriKind.Relative)),
                    ToolTip = item.Item2
                };
                newMarker.Tag = "Matched Location";
                newMarker.Offset = new Point(-15, -30);
                newMarker.ZIndex = 1000;
                _matchedLocations.Add(newMarker);
                Gmap.Markers.Add(newMarker);
                newMarker.Shape.MouseRightButtonUp += _removeMatchedLocation;
            }
        }

        private void _removeMatchedLocation(object sender, MouseButtonEventArgs e)
        {

            if (_matchedLocations != null)
            {
                foreach (var mkr in _matchedLocations)
                {
                    Gmap.Markers.Remove(mkr);
                }
                _matchedLocations.Clear();
            }
        }

        public void UpdateMarkersOnMap()
        {
            Gmap.Markers.Clear();
            if (ShowWECCLocations)
            {
                foreach (var mkr in _filteredWECCMarkerList)
                {
                    Gmap.Markers.Add(mkr);
                }
                //var a = Gmap.Markers.ToList();
                //a.AddRange(_weccMarkerList);
                //Gmap.Markers.Add(a);
            }
            if (ShowPlattsLocations)
            {
                foreach (var mkr in _filteredPlattsMarkerList)
                {
                    Gmap.Markers.Add(mkr);
                }
                //Gmap.Markers.ToList().AddRange(_plattsMarkerList);
            }
            if (ShowEnergyAnalyticsLocations)
            {
                foreach (var mkr in _filteredEnergyAnalyticsMarkerList)
                {
                    Gmap.Markers.Add(mkr);
                }
                //Gmap.Markers.ToList().AddRange(_energyAnalyticsMarkerList);
            }
            if (_foundLocation != null)
            {
                Gmap.Markers.Add(_foundLocation);
            }
            if (_measuredDistanceLine != null)
            {
                Gmap.Markers.Add(_measuredDistanceLine);
            }
            if (_matchedLocations != null)
            {
                foreach (var mkr in _matchedLocations)
                {
                    Gmap.Markers.Add(mkr);
                }
            }
        }
        private void WECCMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as Ellipse;
            WECCMarkerText = s.ToolTip.ToString();
        }
        private void PlattsMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as Ellipse;
            PlattsMarkerText = s.ToolTip.ToString();
        }
        private void ENERGYANAMarker_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var s = sender as Ellipse;
            ENERGYANAMarkerText = s.ToolTip.ToString();
        }
        private string _weccMarcerText;
        public string WECCMarkerText 
        {
            get { return _weccMarcerText; }
            set
            {
                _weccMarcerText = value;
                OnPropertyChanged();
            }
        }
        private string _plattsMarkerText;
        public string PlattsMarkerText
        {
            get { return _plattsMarkerText; }
            set
            {
                _plattsMarkerText = value;
                OnPropertyChanged();
            }
        }
        private string _energyanaMarkerText;
        public string ENERGYANAMarkerText
        {
            get { return _energyanaMarkerText; }
            set
            {
                _energyanaMarkerText = value;
                OnPropertyChanged();
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
        private double _locationToBeFoundX;
        public double LocationToBeFoundX
        {
            set { _locationToBeFoundX = value;
                OnPropertyChanged();
            }
            get { return _locationToBeFoundX; }
        }
        private double _locationToBeFoundY;
        public double LocationToBeFoundY
        {
            set
            {
                _locationToBeFoundY = value;
                OnPropertyChanged();
            }
            get { return _locationToBeFoundY; }
        }
        public ICommand FindLocation { get; set; }
        private GMapMarker _foundLocation;
        private void _findLocation(object obj)
        {
            if (_foundLocation != null)
            {
                Gmap.Markers.Remove(_foundLocation);
            }
            var lc = new PointLatLng(LocationToBeFoundY, LocationToBeFoundX);
            var newMarker = new GMapMarker(lc);
            newMarker.Shape = new Image
            {
                Width = 25,
                Height = 25,
                Source = new BitmapImage(new System.Uri(@"..\MyResources\bigMarkerGreen.png", UriKind.Relative))
            };
            newMarker.Tag = "LocationToBeFound";
            newMarker.Offset = new Point(-12.5, -25);
            _foundLocation = newMarker;
            Gmap.Markers.Add(newMarker);
            Gmap.CenterPosition = lc;
            newMarker.Shape.MouseRightButtonUp += _removeFoundLocation;
            newMarker.Shape.MouseLeftButtonDown += Shape_MouseLeftButtonDown;
            newMarker.Shape.MouseLeftButtonUp += Shape_MouseLeftButtonUp;
            newMarker.Shape.MouseMove += Shape_MouseMove;
        }
        private void Shape_MouseMove(object sender, MouseEventArgs e)
        {
            Image i = sender as Image;
            if (i != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentloc = e.GetPosition(Gmap);
                _foundLocation.Position = Gmap.FromLocalToLatLng((int)currentloc.X, (int)currentloc.Y);
                //DragDrop.DoDragDrop(i, _foundLocation, DragDropEffects.All);
            }
        }
        private void Shape_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            if (i.IsMouseCaptured)
            {
                Mouse.Capture(null);
            }
            var currentloc = e.GetPosition(Gmap);
            var position = Gmap.FromLocalToLatLng((int)currentloc.X, (int)currentloc.Y);
            LocationToBeFoundY = position.Lat;
            LocationToBeFoundX = position.Lng;
        }
        private void Shape_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image i = sender as Image;
            if (!i.IsMouseCaptured)
            {
                Mouse.Capture(i);
            }
        }
        private void _removeFoundLocation(object sender, MouseButtonEventArgs e)
        {
            Gmap.Markers.Remove(_foundLocation);
            _foundLocation = null;
        }
        private bool _showWECCLocations;
        public bool ShowWECCLocations
        {
            get { return _showWECCLocations; }
            set
            {
                if (_showWECCLocations != value)
                {
                    _showWECCLocations = value;
                    OnPropertyChanged();
                    //var sw = new Stopwatch();
                    //sw.Start();
                    //_updateMarkersOnMap();
                    //Debug.WriteLine("Time to toggle WECC: {0}", sw.ElapsedMilliseconds);
                    if (value)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        foreach (var mkr in _filteredWECCMarkerList)
                        {
                            Gmap.Markers.Add(mkr);
                        }
                        //_updateMarkersOnMap();
                        Debug.WriteLine("Time to add WECC: {0}", sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        //for (int i = Gmap.Markers.Count - 1; i >= 0; i--)
                        //{
                        //    if (Gmap.Markers[i].Tag.ToString() == "WECC")
                        //    {
                        //        Gmap.Markers.Remove(Gmap.Markers[i]);
                        //    }
                        //}
                        //Gmap.Markers.Clear();
                        UpdateMarkersOnMap();
                        Debug.WriteLine("Time to delete WECC: {0}", sw.ElapsedMilliseconds);
                    }
                }
            }
        }
        private bool _showPlattsLocations;
        public bool ShowPlattsLocations
        {
            get { return _showPlattsLocations; }
            set
            {
                if (_showPlattsLocations != value)
                {
                    _showPlattsLocations = value;
                    OnPropertyChanged();
                    //var sw = new Stopwatch();
                    //sw.Start();
                    //_updateMarkersOnMap();
                    //Debug.WriteLine("Time to toggle platts: {0}", sw.ElapsedMilliseconds);
                    if (value)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        foreach (var mkr in _filteredPlattsMarkerList)
                        {
                            Gmap.Markers.Add(mkr);
                        }
                        Debug.WriteLine("Time to add platts: {0}", sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        //for (int i = Gmap.Markers.Count - 1; i >= 0; i--)
                        //{
                        //    if (Gmap.Markers[i].Tag.ToString() == "Platts")
                        //    {
                        //        Gmap.Markers.Remove(Gmap.Markers[i]);
                        //    }
                        //}
                        //Gmap.Markers.Clear();
                        UpdateMarkersOnMap();
                        Debug.WriteLine("Time to delete platts: {0}", sw.ElapsedMilliseconds);
                    }
                }
            }
        }
        private bool _showEnergyAnalyticsLocations;
        public bool ShowEnergyAnalyticsLocations
        {
            get { return _showEnergyAnalyticsLocations; }
            set
            {
                if (_showEnergyAnalyticsLocations != value)
                {
                    _showEnergyAnalyticsLocations = value;
                    OnPropertyChanged();
                    //var sw = new Stopwatch();
                    //sw.Start();
                    //_updateMarkersOnMap();
                    //Debug.WriteLine("Time to toggle energy analytics: {0}", sw.ElapsedMilliseconds);
                    if (value)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        foreach (var mkr in _filteredEnergyAnalyticsMarkerList)
                        {
                            Gmap.Markers.Add(mkr);
                        }
                        Debug.WriteLine("Time to add energy analytics: {0}", sw.ElapsedMilliseconds);
                    }
                    else
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        //for (int i = Gmap.Markers.Count - 1; i >= 0; i--)
                        //{
                        //    if (Gmap.Markers[i].Tag.ToString() == "ENERGYANA")
                        //    {
                        //        Gmap.Markers.Remove(Gmap.Markers[i]);
                        //    }
                        //}
                        //Gmap.Markers.Clear();
                        UpdateMarkersOnMap();
                        Debug.WriteLine("Time to delete energy analytics: {0}", sw.ElapsedMilliseconds);
                    }
                }
            }
        }
        private List<GMapMarker> _filteredWECCMarkerList;
        private List<GMapMarker> _filteredPlattsMarkerList;
        private List<GMapMarker> _filteredEnergyAnalyticsMarkerList;
        private List<GMapMarker> _allWECCMarkerList;
        private List<GMapMarker> _allPlattsMarkerList;
        private List<GMapMarker> _allEnergyAnalyticsMarkerList;
        public void ClearEnergyAnalyticsMarkers()
        {
            _filteredEnergyAnalyticsMarkerList.Clear();
            _allEnergyAnalyticsMarkerList.Clear();
            UpdateMarkersOnMap();
            //for (int i = Gmap.Markers.Count - 1; i >=0; i--)
            //{
            //    if (Gmap.Markers[i].Tag.ToString() == "ENERGYANA")
            //    {
            //        Gmap.Markers.Remove(Gmap.Markers[i]);
            //    }
            //}
        }
        public void ClearWECCMarkers()
        {
            _filteredWECCMarkerList.Clear();
            _allWECCMarkerList.Clear();
            UpdateMarkersOnMap();
            //for (int i = Gmap.Markers.Count - 1; i >= 0; i--)
            //{
            //    if (Gmap.Markers[i].Tag.ToString() == "WECC")
            //    {
            //        Gmap.Markers.Remove(Gmap.Markers[i]);
            //    }
            //}
        }
        public void ClearPlattsMarkers()
        {
            _filteredPlattsMarkerList.Clear();
            _allPlattsMarkerList.Clear();
            UpdateMarkersOnMap();
            //for (int i = Gmap.Markers.Count - 1; i >= 0; i--)
            //{
            //    if (Gmap.Markers[i].Tag.ToString() == "Platts")
            //    {
            //        Gmap.Markers.Remove(Gmap.Markers[i]);
            //    }
            //}
        }
        public delegate void MyEvent();
        public event MyEvent NewMarkersCreated;
        protected virtual void OnNewMarkersCreated()
        {
            NewMarkersCreated?.Invoke();
        }
        public void SetFilteredMarkers(List<GMapMarker> weccMarkers = null, List<GMapMarker> plattsMarkers = null, List<GMapMarker> energyAnalyticsMarkers = null)
        {
            if (weccMarkers == null)
            {
                _filteredWECCMarkerList = _allWECCMarkerList;
            }
            else
            {
                _filteredWECCMarkerList = weccMarkers;
            }
            if (plattsMarkers == null)
            {
                _filteredPlattsMarkerList = _allPlattsMarkerList;
            }
            else
            {
                _filteredPlattsMarkerList = plattsMarkers;
            }
            if (energyAnalyticsMarkers == null)
            {
                _filteredEnergyAnalyticsMarkerList = _allEnergyAnalyticsMarkerList;
            }
            else
            {
                _filteredEnergyAnalyticsMarkerList = energyAnalyticsMarkers;
            }
        }

    }
}
