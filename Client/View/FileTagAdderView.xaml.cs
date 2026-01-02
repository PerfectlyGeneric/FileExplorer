using System;
using FileExplorer.ViewModel;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;

namespace FileExplorer.View
{
    /// <summary>
    /// Interaction logic for FileTagAdder.xaml
    /// </summary>
    public partial class FileTagAdderView : Window
    {
        public FileTagAdderView(String name, ObservableCollection<string> fileTags)
        {
            InitializeComponent();

            DataContext = new FileTagAdderViewModel(name, fileTags, (result) =>
            {
                this.DialogResult = result;
                this.Close();
            });

        }

        public FileTagAdderView(List<File> selectedFiles, IEnumerable<string> globalTags)
        {
            InitializeComponent();

            DataContext = new FileTagAdderViewModel(selectedFiles, globalTags, (result) =>
            {
                this.DialogResult = result;
                this.Close();
            });
        }
    }
}
