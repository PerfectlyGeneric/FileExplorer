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

            //Action closeAction = new Action(() => this.Close());

            this.DataContext = new FileDetailsWindowModel(aiService, file, globalTags,
                (result) => { this.DialogResult = result; this.Close(); });
        }

    }
}
