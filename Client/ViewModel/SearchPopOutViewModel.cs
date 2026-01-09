using FileExplorer.CommonHelpers;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FileExplorer.ViewModel
{
    internal class SearchPopOutViewModel : CommonHelpers.ViewModelBase
    {
        public ICommand TypeTagsCommand {  get; set; }
        public ICommand TypeNotTagsCommand { get; set; }
        public ICommand TypeNamesCommand { get; set; }
        public ICommand TypeNotNamesCommand { get; set; }
        public ICommand CloseCommand { get; set; }
        public ICommand ExecuteSearchCommand { get; set; }

        private ObservableCollection<string> _tags;
        public ObservableCollection<string> Tags {
            get => _tags;
            set => SetProperty(ref _tags, value);
        }

        private ObservableCollection<string> _notTags;
        public ObservableCollection<string> NotTags
        {
            get => _notTags;
            set => SetProperty(ref _notTags, value);
        }

        private ObservableCollection<string> _names;
        public ObservableCollection<string> Names
        {
            get => _names;
            set => SetProperty(ref _names, value);
        }
        private ObservableCollection<string> _notNames;
        public ObservableCollection<string> NotNames
        {
            get => _notNames;
            set => SetProperty(ref _notNames, value);
        }

        public ObservableCollection<File> SortedFiles;

        public List<File> givenFiles;
        public HashSet<string> givenTags;
        private Action<bool> _closeAction;

        private ObservableCollection<string> _cuteDisplay;
        public ObservableCollection<string> CuteDisplay
        {
            get => _cuteDisplay;
            set => SetProperty(ref _cuteDisplay, value);
        }


        public SearchPopOutViewModel(IEnumerable<File> sourceFiles, HashSet<string> globalTags, Action<bool> closeAction)
        {
            givenFiles = sourceFiles.ToList();
            givenTags = globalTags;
            _closeAction = closeAction;

            Tags = new ObservableCollection<string>();
            NotTags = new ObservableCollection<string>();
            Names = new ObservableCollection<string>();
            NotNames = new ObservableCollection<string>();

            CuteDisplay = new ObservableCollection<string>() { "Add Search Options"};

            TypeTagsCommand = new CommonHelpers.RelayCommand<string>(AddTags);
            TypeNamesCommand = new CommonHelpers.RelayCommand<string>(AddNames);
            TypeNotTagsCommand = new CommonHelpers.RelayCommand<string>(AddNotTags);
            TypeNotNamesCommand = new CommonHelpers.RelayCommand<string>(AddNotNames);
            ExecuteSearchCommand = new CommonHelpers.RelayCommand(ExecuteSearch);
            CloseCommand = new CommonHelpers.RelayCommand(ExecuteClose);
        }

        private void ExecuteClose(object CloseGiven)
        {
            _closeAction?.Invoke(false);
        }

        private void AddTags(string userString)
        {
            if (!string.IsNullOrWhiteSpace(userString))
            {
                var splitString = userString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => t.Trim());

                foreach (var tag in splitString)
                {
                    if (!Tags.Contains(tag))
                        Tags.Add(tag);
                }
            }

            UpdateCuteDisplay();
        }

        private void AddNotTags(string userString)
        {
            if (!string.IsNullOrWhiteSpace(userString))
            {
                var splitString = userString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => t.Trim());

                NotTags = new ObservableCollection<string>(splitString);
            }

            UpdateCuteDisplay();
        }

        private void AddNotNames(string userString)
        {
            if (!string.IsNullOrWhiteSpace(userString))
            {
                var splitString = userString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => t.Trim());

                NotNames = new ObservableCollection<string>(splitString);
            }

            UpdateCuteDisplay();
        }

        private void AddNames(string userString)
        {
            if (!string.IsNullOrWhiteSpace(userString))
            {
                var splitString = userString.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(t => t.Trim());

                Names = new ObservableCollection<string>(splitString);
            }

            UpdateCuteDisplay();
        }

        private void ExecuteSearch(object SearchGiven)
        {
            SortedFiles = FileAlgorithms.BasicCombinedSearch(givenFiles, CuteDisplay);

            _closeAction?.Invoke(true);

        }

        //This will show the Search String being updated in real time
        private void UpdateCuteDisplay()
        {
            var validSegments = new List<string>();

            void AddSegment(IEnumerable<string> source, string prefix, bool isExclusion)
            {
                if (source != null && source.Any())
                {
                    string joinedItems = string.Join(", ", source);

                    string logicalOperator = isExclusion ? "!" : "";
                    validSegments.Add($"{logicalOperator}({prefix}: {joinedItems})");
                }
            }

            // true = adds '!'
            AddSegment(Names, "name", false);
            AddSegment(NotNames, "name", true);
            AddSegment(Tags, "tag", false);
            AddSegment(NotTags, "tag", true);

            CuteDisplay.Clear();

            if (validSegments.Count > 0)
            {
                string finalString = $"[{string.Join(" && ", validSegments)}]";
                CuteDisplay.Add(finalString);
            }
            else
            {
                CuteDisplay.Add("Add Search Options");
            }
        }

    }
}
