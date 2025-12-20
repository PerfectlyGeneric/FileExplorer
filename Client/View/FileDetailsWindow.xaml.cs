using AplicationUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AplicationUI.View
{
    /// <summary>
    /// Interaction logic for FileDetailsWindow.xaml
    /// </summary>
    public partial class FileDetailsWindow : Window
    {
        File file;
        public FileDetailsWindow(File file, AiModelHandler aiService, IEnumerable<string> globalTags)
        {
            InitializeComponent();
            this.file = file;
            this.DataContext = new FileDetailsWindowModel(aiService, file, globalTags);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
