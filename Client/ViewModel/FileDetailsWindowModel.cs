using FileExplorer.ViewModel;
using FileExplorer.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace FileExplorer.ViewModel
{
    internal class FileDetailsWindowModel : CommonHelpers.ViewModelBase
    {
        private readonly AiModelHandler _aiService;
        private readonly File _originalFile;
        private readonly IEnumerable<string> _globalTags;
        private readonly IFileDetailsInterface _currentPreview;

        private Action<bool> _closeAction;
        public ICommand SaveCommand { get; }
        public ICommand RemoveTagCommand { get; }
        public ICommand AddSuggestedTagCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand AddTagCommand { get; }

        public File File => _originalFile;
        public IFileDetailsInterface CurrentPreview => _currentPreview;

        /*private string _yourTags;
        public string YourTags
        {
            get => _yourTags;
            set => SetProperty(ref _yourTags, value);
        }

        private string _suggestedTags;
        public string SuggestedTags
        {
            get => _suggestedTags;
            set => SetProperty(ref _suggestedTags, value);


        }*/

        private ObservableCollection<string> _yourTags;
        public ObservableCollection<string> YourTags
        {
            get => _yourTags;
            set => SetProperty(ref _yourTags, value);
        }

        private ObservableCollection<string> _suggestedTags;
        public ObservableCollection<string> SuggestedTags
        {
            get => _suggestedTags;
            set => SetProperty(ref _suggestedTags, value);
        }

        private string _notes;
        public string Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }


        public FileDetailsWindowModel(AiModelHandler aiService, File file, IEnumerable<string> globalTags, Action<bool> closeAction)
        {
            _originalFile = file;
            _aiService = aiService;
            _globalTags = globalTags;
            _closeAction = closeAction;


            YourTags = new ObservableCollection<string>(file.Relic.Tags ?? new List<string>());
            Notes = _originalFile.Relic.Notes;

            //replace logic
            if (file.GetFileType() == "Image") _currentPreview = new ImagePreview(file);
            else _currentPreview = new DefaultPreview(file);

            SaveCommand = new CommonHelpers.RelayCommand(ExecuteSave);
            RemoveTagCommand = new CommonHelpers.RelayCommand<string>(RemoveTag);
            AddSuggestedTagCommand = new CommonHelpers.RelayCommand<string>(AddSuggestedTag);
            CloseCommand = new CommonHelpers.RelayCommand(ExecuteClose);
            AddTagCommand = new CommonHelpers.RelayCommand(AddTag);


            ProcessSelectedFileAsync();
        }

        private void ExecuteClose(object CloseGiven)
        {
            _closeAction?.Invoke(false);
        }

        private void ExecuteSave(object SaveGiven)
        {
            _originalFile.Relic.Notes = Notes;
            _originalFile.Relic.Tags = YourTags.ToList();

            _closeAction?.Invoke(true);
        }

        /// <summary>
        /// Saves the current state of the file, including notes and tags, and closes the associated window if provided.    
        /// </summary>
        /// <remarks>This method updates the file's notes and tags based on the current state. Tags are
        /// parsed from a separator string, removing any empty entries. If no tags are provided, the
        /// file's tags are set to an empty list.</remarks>
        /// <param name="SaveGiven">An object representing the window to close after saving. </param>
        /*private void ExecuteSave(object SaveGiven)
        {
            _originalFile.Relic.Notes = Notes;

            if (!string.IsNullOrWhiteSpace(YourTags))
            {
                var tags = YourTags.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(tag => tag.Trim())
                                   .ToList();
                _originalFile.Relic.Tags = tags;
            }
            else
            {
                _originalFile.Relic.Tags = new List<string>();
            }

            if (SaveGiven is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }

        }*/

        /// <summary>
        /// Converts the tags associated with the specified file into a single comma-separated string.  
        /// </summary>
        /// <param name="file">The file whose tags are to be converted. The file must not be null.</param>
        /// <returns>A comma-separated string containing the tags if the file has any tags; otherwise, an empty string.</returns>
        private string ConvertToSeparateStrings(File file)
        {
            if (file.Relic.Tags != null && file.Relic.Tags.Count > 0)
            {
                return string.Join(", ", file.Relic.Tags);
            }
            else
            {
                return string.Empty;
            }
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
            var detailsWindow = new FileTagAdderView(File.Name, YourTags);

            bool? result = detailsWindow.ShowDialog();

            if (result == true)
            {
                if (detailsWindow.DataContext is FileTagAdderViewModel childvm)
                {
                    YourTags = new ObservableCollection<string>(childvm.Tags);
                }
            }
        }

        private void AddSuggestedTag(string tag)
        {
            if (!YourTags.Contains(tag))
            {
                YourTags.Add(tag);
                if (SuggestedTags.Contains(tag))
                {
                    SuggestedTags.Remove(tag);
                    ProcessSelectedFileAsync();
                }
            }
        }

        /// <summary>
        /// Processes the currently selected file and updates the suggested tags based on AI-generated recommendations. 
        /// </summary>
        /// <remarks>This method retrieves the file path of the selected file, determines candidate tags.</remarks>
        private async void ProcessSelectedFileAsync()
        {
            if (File == null) return;

            try
            {
                //SuggestedTags = "Processing...";

                var currentTags = _originalFile.Relic.Tags ?? new List<string>();
                var candidateTags = _globalTags.Except(currentTags).ToList();

                string choice = File.GetPath();

                string result = await _aiService.GetAIResponse(choice, candidateTags);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    var splitTags = result.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(t => t.Trim());

                    SuggestedTags = new ObservableCollection<string>(splitTags);
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error processing file: {ex.Message}");
            }
        }
    }
}
