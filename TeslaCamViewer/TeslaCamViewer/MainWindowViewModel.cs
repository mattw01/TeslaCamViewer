﻿

namespace TeslaCamViewer
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TeslaCamDirectoryCollection> ListItems { get; set; }
        public TeslaCamFileSet CurrentPlaybackFile { get; set; }

        private GridLength _TopVideoRowHeight;
        public GridLength TopVideoRowHeight
        {
            get
            {
                return this._TopVideoRowHeight;
            }
            set
            {
                if (value != this._TopVideoRowHeight)
                {
                    this._TopVideoRowHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private GridLength _BottomVideoRowHeight;
        public GridLength BottomVideoRowHeight
        {
            get
            {
                return this._BottomVideoRowHeight;
            }
            set
            {
                if (value != this._BottomVideoRowHeight)
                {
                    this._BottomVideoRowHeight = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private GridLength _LeftVideoColumnWidth;
        public GridLength LeftVideoColumnWidth
        {
            get
            {
                return this._LeftVideoColumnWidth;
            }
            set
            {
                if (value != this._LeftVideoColumnWidth)
                {
                    this._LeftVideoColumnWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private GridLength _RightVideoColumnWidth;
        public GridLength RightVideoColumnWidth
        {
            get
            {
                return this._RightVideoColumnWidth;
            }
            set
            {
                if (value != this._RightVideoColumnWidth)
                {
                    this._RightVideoColumnWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private double _DisplayPlaybackSpeed;
        public double DisplayPlaybackSpeed
        {
            get
            {
                return this._DisplayPlaybackSpeed;
            }
            set
            {
                if (value != this._DisplayPlaybackSpeed)
                {
                    this._DisplayPlaybackSpeed = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("CalculatedPlaybackSpeed");
                }
            }
        }
        public double CalculatedPlaybackSpeed
        {
            get
            {
                double calculatedMax = 1.00;

                if (DisplayPlaybackSpeed < 0)
                {
                    double calculatedMin = 0.25;
                    double displayMin = -20;
                    double displayMax = 0;

                    double calc = calculatedMax - (calculatedMax - calculatedMin) / (displayMax - displayMin) * (displayMax - this.DisplayPlaybackSpeed);
                    return Math.Round(Math.Floor(calc * 20.0) / 20, 2);
                }
                else
                    return Math.Round((int)(this.DisplayPlaybackSpeed) / 2.0, 1) + calculatedMax;
            }
            set
            {
            }
        }

        private string _LeftStatusText;
        public string LeftStatusText
        {
            get
            {
                return this._LeftStatusText;
            }

            set
            {
                if (value != this._LeftStatusText)
                {
                    this._LeftStatusText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _RightStatusText;
        public string RightStatusText
        {
            get
            {
                return this._RightStatusText;
            }

            set
            {
                if (value != this._RightStatusText)
                {
                    this._RightStatusText = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool EnableAutoSearch
        {
            get
            {
                return Properties.Settings.Default.EnableAutoSearch;
            }
            set
            {
                Properties.Settings.Default.EnableAutoSearch = value;
                Properties.Settings.Default.Save();
            }
        }
        public bool EnableAutoPlaylist
        {
            get
            {
                return Properties.Settings.Default.EnableAutoPlaylist;
            }
            set
            {
                Properties.Settings.Default.EnableAutoPlaylist = value;
                Properties.Settings.Default.Save();
            }
        }
        public VideoViewModel VideoModel { get; set; }
        public MainWindowViewModel()
        {
            this.ListItems = new ObservableCollection<TeslaCamDirectoryCollection>();
            this.VideoModel = new VideoViewModel();

            ResetVideoDisplay();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public ICommand SelectFullVideo { get { return new DelegateCommand(DisplayFullVideo); } }
        private TeslaCamFile.CameraType CurrentFullVideo;
        private void DisplayFullVideo(object Camera)
        {
            if (Camera is TeslaCamFile.CameraType)
            {
                if (CurrentFullVideo == TeslaCamFile.CameraType.UNKNOWN)
                {
                    var cam = (TeslaCamFile.CameraType)Camera;
                    if (cam == TeslaCamFile.CameraType.FRONT)
                    {
                        this.TopVideoRowHeight = new GridLength(1, GridUnitType.Star);
                        this.BottomVideoRowHeight = new GridLength(0);
                        this.LeftVideoColumnWidth = new GridLength(1, GridUnitType.Star);
                        this.RightVideoColumnWidth = new GridLength(0);
                    }
                    if (cam == TeslaCamFile.CameraType.BACK)
                    {
                        this.TopVideoRowHeight = new GridLength(1, GridUnitType.Star);
                        this.BottomVideoRowHeight = new GridLength(0);
                        this.LeftVideoColumnWidth = new GridLength(0);
                        this.RightVideoColumnWidth = new GridLength(1, GridUnitType.Star);
                    }
                    if (cam == TeslaCamFile.CameraType.LEFT_REPEATER)
                    {
                        this.TopVideoRowHeight = new GridLength(0);
                        this.BottomVideoRowHeight = new GridLength(1, GridUnitType.Star);
                        this.LeftVideoColumnWidth = new GridLength(1, GridUnitType.Star);
                        this.RightVideoColumnWidth = new GridLength(0);
                    }
                    if (cam == TeslaCamFile.CameraType.RIGHT_REPEATER)
                    {
                        this.TopVideoRowHeight = new GridLength(0);
                        this.BottomVideoRowHeight = new GridLength(1, GridUnitType.Star);
                        this.LeftVideoColumnWidth = new GridLength(0);
                        this.RightVideoColumnWidth = new GridLength(1, GridUnitType.Star);
                    }
                    CurrentFullVideo = cam;
                }
                else
                {
                    ResetVideoDisplay();
                    CurrentFullVideo = TeslaCamFile.CameraType.UNKNOWN;
                }
            }
        }
        private void ResetVideoDisplay()
        {
            this.TopVideoRowHeight = new GridLength(1, GridUnitType.Star);
            this.BottomVideoRowHeight = new GridLength(1, GridUnitType.Star);
            this.LeftVideoColumnWidth = new GridLength(1, GridUnitType.Star);
            this.RightVideoColumnWidth = new GridLength(1, GridUnitType.Star);
        }
        public void LoadFileSet(TeslaCamFileSet set)
        {
            this.VideoModel.LoadFileSet(set);
            this.CurrentPlaybackFile = set;
        }
    }
}
