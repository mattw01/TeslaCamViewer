using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TeslaCamViewer
{

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TeslaCamDirectoryCollection> ListItems { get; set; }

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

        public MainWindowViewModel()
        {
            this.ListItems = new ObservableCollection<TeslaCamDirectoryCollection>();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel model;
        private TimeSpan TotalTime;
        private bool paused;

        public MainWindow()
        {
            this.model = new MainWindowViewModel();
            this.DataContext = model;

            this.model.LeftStatusText = "Ready";
            InitializeComponent();
        }


        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            TotalTime = left.NaturalDuration.TimeSpan;
            var timerVideoTime = new DispatcherTimer();
            timerVideoTime.Interval = TimeSpan.FromMilliseconds(100);
            timerVideoTime.Tick += new EventHandler(timer_Tick);
            timerVideoTime.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (left.NaturalDuration.HasTimeSpan)
                if (left.NaturalDuration.TimeSpan.TotalSeconds > 0)
                    if (TotalTime.TotalSeconds > 0)
                    {
                        model.RightStatusText = left.Position.ToString(@"mm\:ss") + " / " + TotalTime.ToString(@"mm\:ss");
                        timeSlider.Value = left.Position.TotalSeconds / TotalTime.TotalSeconds;
                    }
        }
        private void SetPosition()
        {
            if (TotalTime.TotalSeconds > 0)
            {
                left.Position = TimeSpan.FromSeconds(timeSlider.Value * TotalTime.TotalSeconds);
                right.Position = TimeSpan.FromSeconds(timeSlider.Value * TotalTime.TotalSeconds);
                front.Position = TimeSpan.FromSeconds(timeSlider.Value * TotalTime.TotalSeconds);
            }
        }

        private void timeSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            left.Pause();
            right.Pause();
            front.Pause();
        }

        private void timeSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            left.Play();
            right.Play();
            front.Play();
        }

        private void timeSlider_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            this.SetPosition();
        }
        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
                if (treeview.SelectedItem is TeslaCamFileSet)
                {
                    var set = treeview.SelectedItem as TeslaCamFileSet;
                    LoadFileSet(set);
                }
            }

        }
        private void LoadFileSet(TeslaCamFileSet set)
        {
            left.Stop();
            right.Stop();
            front.Stop();
            bool playLeft = false;
            bool playRight = false;
            bool playFront = false;
            foreach (var cam in set.Cameras)
            {
                if (cam.CameraLocation == TeslaCamFile.CameraType.FRONT)
                {
                    this.front.Source = new Uri(cam.FilePath);
                    playFront = true;
                }
                if (cam.CameraLocation == TeslaCamFile.CameraType.LEFT_REPEATER)
                {
                    this.left.Source = new Uri(cam.FilePath);
                    playLeft = true;
                }
                if (cam.CameraLocation == TeslaCamFile.CameraType.RIGHT_REPEATER)
                {
                    this.right.Source = new Uri(cam.FilePath);
                    playRight = true;
                }
            }

            if (playLeft) left.Play();
            if (playRight) right.Play();
            if (playFront) front.Play();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                var f = files[0];
                FileAttributes attr = File.GetAttributes(f);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    this.model.ListItems.Clear();
                    var c = new TeslaCamDirectoryCollection();
                    c.BuildFromBaseDirectory(f);
                    this.model.ListItems.Add(c);
                    this.model.LeftStatusText = "Location: " + f;
                }
            }
        }
        private void TeslaCamSearch()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                     new Action(() =>
                     { this.model.LeftStatusText = "Searching for TeslaCam ..."; }));
                var drives = System.IO.DriveInfo.GetDrives();
                bool driveFound = false;
                foreach (var drive in drives)
                {
                    var rootDirs = drive.RootDirectory.GetDirectories();
                    foreach (var dirs in rootDirs)
                    {
                        if (dirs.Name == "TeslaCam")
                        {
                            var RecentClipsDir = dirs.GetDirectories().FirstOrDefault(e => e.Name == "RecentClips");
                            if (RecentClipsDir != null)
                            {
                                TeslaCamDirectoryCollection c = new TeslaCamDirectoryCollection();
                                c.BuildFromBaseDirectory(RecentClipsDir.FullName);
                                c.SetDisplayName("Recent Clips");
                                this.model.ListItems.Add(c);
                                driveFound = true;
                            }
                            var SavedClipsDir = dirs.GetDirectories().FirstOrDefault(e => e.Name == "SavedClips");
                            if (SavedClipsDir != null)
                            {
                                TeslaCamDirectoryCollection c = new TeslaCamDirectoryCollection();
                                c.BuildFromBaseDirectory(SavedClipsDir.FullName);
                                c.SetDisplayName("Saved Clips");
                                this.model.ListItems.Add(c);
                                driveFound = true;
                            }
                        }
                        if (driveFound)
                        {
                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                new Action(() =>
                                { this.model.LeftStatusText = "Location: " + dirs.FullName; }));

                            return;
                        }
                    }
                }
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    new Action(() =>
                    {
                        this.model.LeftStatusText = "Ready";
                        this.ShowMessageAsync("TeslaCam Drive Not Found", "A TeslaCam drive could not automatically be found. Drag a folder or file to start playing.");
                    }));

            });
        }

        private void teslaCamSearch_Menu_Click(object sender, RoutedEventArgs e)
        {
            this.model.ListItems.Clear();
            TeslaCamSearch();
        }

        private void playPause_Button_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                left.Play();
                right.Play();
                front.Play();
            }
            else
            {
                left.Pause();
                right.Pause();
                front.Pause();
            }
            paused = !paused;
        }

        private void exit_Menu_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                playPause_Button_Click(sender, null);
            if (e.Key == Key.F)
            {
                fullscreen_Menu.IsChecked = !fullscreen_Menu.IsChecked;
                SetFullscreen(fullscreen_Menu.IsChecked);
            }
            if (e.Key == Key.Escape)
            {
                if (fullscreen_Menu.IsChecked)
                {
                    fullscreen_Menu.IsChecked = !fullscreen_Menu.IsChecked;
                    SetFullscreen(fullscreen_Menu.IsChecked);
                }
            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (model.EnableAutoSearch)
                this.TeslaCamSearch();
        }

        private void about_Menu_Click(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("TeslaCam Viewer V0.1", "TeslaCam Viewer V0.1 Copyright 2019 mattw\n\nSee LICENCES.txt for more information.");
        }

        private void viewOnGitHub_Menu_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/mattw01/TeslaCamViewer/");
        }

        private void SetFullscreen(bool Enable)
        {
            if (Enable)
            {
                this.SetCurrentValue(IgnoreTaskbarOnMaximizeProperty, true);
                this.SetCurrentValue(WindowStateProperty, WindowState.Maximized);
                this.SetCurrentValue(UseNoneWindowStyleProperty, true);
            }
            else
            {
                this.SetCurrentValue(WindowStateProperty, WindowState.Normal);
                this.SetCurrentValue(UseNoneWindowStyleProperty, false);
                this.SetCurrentValue(ShowTitleBarProperty, true); // <-- this must be set to true
                this.SetCurrentValue(IgnoreTaskbarOnMaximizeProperty, false);
            }
        }

        private void fullscreen_Menu_Click(object sender, RoutedEventArgs e)
        {
            SetFullscreen(fullscreen_Menu.IsChecked);
        }
        private void ShowWelcomeMessage()
        {
            this.ShowMessageAsync("Welcome to TeslaCam Viewer!", "Getting Started:\n\nBrowse TeslaCam media in the left pane. " + 
                "TeslaCam drive will automatically be detected on startup, or drag a folder containing TeslaCam data anywhere onto the window. " + 
                "Double click event in TeslaCam Files pane to start playing.");
        }
    }
}
