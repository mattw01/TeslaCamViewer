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
    public partial class RootCollectionView : Page
    {
        private MainWindowViewModel mainModel;
        public RootCollectionView(MainWindowViewModel model)
        {
            this.mainModel = model;
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
                if (listBox.SelectedItem is TeslaCamDirectoryCollection)
                {
                    var set = listBox.SelectedItem as TeslaCamDirectoryCollection;
                    this.NavigationService.Navigate(new DirectoryCollectionView(set, mainModel));
                }
            }

        }
    }
}
