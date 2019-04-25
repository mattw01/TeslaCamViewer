using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace TeslaCamViewer.Views
{
    public class EventCollectionViewModel : INotifyPropertyChanged
    {


        private int _ThumbnailSize;
        public int ThumbnailSize
        {
            get
            {
                return this._ThumbnailSize;
            }

            set
            {
                if (value != this._ThumbnailSize)
                {
                    this._ThumbnailSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public TeslaCamEventCollection EventData { get; set; }

        public EventCollectionViewModel()
        {
            this.ThumbnailSize = 250;
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
    /// Interaction logic for DirectoryCollectionView.xaml
    /// </summary>
    public partial class EventCollectionView : Page
    {
        MainWindowViewModel mainModel;
        EventCollectionViewModel model;
        public EventCollectionView(TeslaCamEventCollection data, MainWindowViewModel mainModel)
        {
            this.model = new EventCollectionViewModel();
            model.EventData = data;

            this.mainModel = mainModel;
            this.DataContext = model;
            InitializeComponent();
        }
        private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is ListBoxItem)
            {
                if (!((ListBoxItem)sender).IsSelected)
                {
                    return;
                }
                if (listBox.SelectedItem is TeslaCamFileSet)
                {
                    var set = listBox.SelectedItem as TeslaCamFileSet;
                    mainModel.LoadFileSet(set);
                    
                }
            }

        }

        private void MediaElement_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is MediaElement)
            {
                var element = sender as MediaElement;
                var position = e.GetPosition(element);
                var percent = position.X / element.Width;
                if (element.NaturalDuration.HasTimeSpan)
                {
                    var total = element.NaturalDuration.TimeSpan.TotalSeconds;
                    var totalSeconds = Convert.ToInt32(total * percent);
                    element.Position = new TimeSpan(0, 0, 0, totalSeconds);
                }
            }
        }

        private void goBack_Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
                this.NavigationService.GoBack();
        }
    }
}
