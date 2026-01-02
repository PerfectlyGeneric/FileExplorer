using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FileExplorer.ViewModel;
using FileExplorer.View;

namespace FileExplorer.ViewModel
{
    public class SingleFileMode { }
    public class MultipleFilesMode { }

    internal class FileTagAdderViewModel : CommonHelpers.ViewModelBase
    {
        private readonly List<String>? _commonTags;
        private object _currentPreview;
        private string _inputTag;

        public string GivenTitle { get; set; }
        public string InputTag { get => _inputTag; set => SetProperty(ref _inputTag, value); }


        private readonly IEnumerable<string> _globalTags;

        private Action<bool> _closeAction;
        public ICommand SaveCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddTagCommand { get; }

        public ObservableCollection<string> Tags;
        private ObservableCollection<string> _yourTags;
        public ObservableCollection<string> YourTags
        {
            get => _yourTags;
            set => SetProperty(ref _yourTags, value);
        }

        public object CurrentPreview
        {
            get => _currentPreview;
            set => SetProperty(ref _currentPreview, value);
        }

        public FileTagAdderViewModel(string Name, ObservableCollection<string> Tags, Action<bool> closeAction)
        {
            GivenTitle = Name;
            this.Tags = new ObservableCollection<string>();
            YourTags = new ObservableCollection<string>(Tags ?? new ObservableCollection<string>());

            CurrentPreview = new SingleFileMode();
            _closeAction = closeAction;


            RemoveTagCommand = new CommonHelpers.RelayCommand<string>(RemoveTag);
            AddTagCommand = new CommonHelpers.RelayCommand(AddTag);
            SaveCommand = new CommonHelpers.RelayCommand(ExecuteShallowSave);
            CloseCommand = new CommonHelpers.RelayCommand(ExecuteClose);
        }

        //Future Implementation for when click selecting multiple files
        public FileTagAdderViewModel(List<String> GlobalTags, List<File> SelectedFiles)
        {
            _globalTags = GlobalTags;
            CurrentPreview = new MultipleFilesMode();
            

            //We'll execute save command here, no other menu to fall back on for confirmation in this case
        }

        //Will only get called when 
        private void ExecuteSave(object SaveGiven)
        {
            //Do Later
            
        }

        private void ExecuteClose(object CloseGiven)
        {
            _closeAction?.Invoke(false);
        }

        private void ExecuteShallowSave(object SaveGiven)
        {
            Tags = YourTags;
            _closeAction?.Invoke(true);
        }

        private void RemoveTag(string tag)
        {
            if (YourTags.Contains(tag))
            {
                YourTags.Remove(tag);
            }
        }

        private void AddTag(object filler)
        {
            var text = InputTag?.Trim();
            if (!YourTags.Contains(text) && !string.IsNullOrWhiteSpace(text))
            {
                YourTags.Add(text);
                InputTag = string.Empty;


            }
        }
    }
}
