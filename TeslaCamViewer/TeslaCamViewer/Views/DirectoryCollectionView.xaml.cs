using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TeslaCamViewer.Views
{
    /// <summary>
    /// Interaction logic for DirectoryCollectionView.xaml
    /// </summary>
    public partial class DirectoryCollectionView : Page
    {
        private MainWindowViewModel mainModel;
        public DirectoryCollectionView(TeslaCamDirectoryCollection data, MainWindowViewModel mainModel)
        {
            this.mainModel = mainModel;
            this.DataContext = data;
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
                if (listBox.SelectedItem is TeslaCamEventCollection)
                {
                    var set = listBox.SelectedItem as TeslaCamEventCollection;
                    this.NavigationService.Navigate(new EventCollectionView(set, mainModel));
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
