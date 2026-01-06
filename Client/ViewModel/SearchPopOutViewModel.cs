using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModel
{
    internal class SearchPopOutViewModel : CommonHelpers.ViewModelBase
    {

        public SearchPopOutViewModel(IEnumerable<File> sourceFiles, IEnumerable<string> globalTags, Action<bool> closeAction)
        {

        }
    }
}
