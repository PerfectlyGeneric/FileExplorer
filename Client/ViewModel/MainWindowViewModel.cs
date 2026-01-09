using FileExplorer.CommonHelpers;
using FileExplorer.View;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace FileExplorer.ViewModel
{
    internal class MainWindowViewModel : CommonHelpers.ViewModelBase
    {

        public ICommand SelectFolderCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        public ICommand SortCommand { get; private set; }

        private readonly AiModelHandler _aiService;
        private List<File> _allFiles;
        private HashSet<string> _globalTags;

        private ObservableCollection<File> _populatedFiles;
        public ObservableCollection<File> PopulatedFiles
        {
            get => _populatedFiles;
            set => SetProperty(ref _populatedFiles, value);
        }
        public ObservableCollection<File> SavedFiles { get; set; }
        private ObservableCollection<File> _inputFiles;
        public ObservableCollection<File> InputFiles
        {
            get => _inputFiles;
            set => SetProperty(ref _inputFiles, value);
        }

        private File? _selectedFile;
        public File? SelectedFile
        {
            get => _selectedFile;
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    // No longer needed, changed in favour of DoubleClick event
                    //ProcessSelectedFileAsync();
                    //ShowFileDetailsPopup(value);
                }
            }
        }

        private string _resultText;
        public string ResultText
        {
            get => _resultText;
            set => SetProperty(ref _resultText, value);
        }

        private string _selectedFolderPath;
        public string SelectedFolderPath
        {
            get => _selectedFolderPath;
            set
            {
                if (SetProperty(ref _selectedFolderPath, value))
                {
                    LoadFilesFromFolder(value);
                }
            }
        }

        public MainWindowViewModel()
        {
            AiModelHandler.GetModelUp();

            var httpClinet = new HttpClient();
            _aiService = new AiModelHandler(httpClinet);

            SelectFolderCommand = new CommonHelpers.RelayCommand(ExecuteSelectFolder);
            OpenFileCommand = new CommonHelpers.RelayCommand(ExecuteOpenFile);
            AddTagCommand = new CommonHelpers.RelayCommand(ExecuteAddTag);
            SortCommand = new CommonHelpers.RelayCommand<string>(ExecuteSort);


            InputFiles = new ObservableCollection<File>();
            SavedFiles = new ObservableCollection<File>();
            _globalTags = new HashSet<string>();    
        }

        private void ExecuteSort(string obj)
        {
            switch (obj)
            {
                case "Recent":
                    PopulatedFiles = SavedFiles;
                    break;
                case "Favourites":
                    //SortFav(); Implement Later when Fav tags are added
                    break;
                case "Clear":
                    PopulatedFiles = null;
                    break;
                case "Search":
                    PopOutSearch();
                    break;
            }
        }

        /// <summary>
        /// Loads files from the specified folder and updates the collection of input files.
        /// </summary>
        /// <remarks>If the specified folder does not exist or the path is null or empty, the method does
        /// nothing. The loaded files are stored internally and exposed through the <see cref="InputFiles"/>
        /// property.</remarks>
        /// <param name="folderPath">The path to the folder containing the files to load. Must be a valid, existing directory.</param>
        public void LoadFilesFromFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
                return;

            var files = PathHandler.LoadFileDataGiven(folderPath);
            _allFiles = files;
            InputFiles = new ObservableCollection<File>(files);
        }

        /// <summary>
        /// Method to open a folder selection dialog, default from Windows.
        /// </summary>
        /// <param name="parameter"></param>
        /// <remarks>
        /// 
        /// </remarks>
        private void ExecuteSelectFolder(object parameter)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;

                CommonFileDialogResult result = dialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok)
                {
                    SelectedFolderPath = dialog.FileName;
                }
            }
        }

        /// <summary>
        /// Shows the File Details popup window.
        /// </summary>
        /// <param name="file">The file to show details for.</param>
        /// <remarks>
        /// Calls the FileDetailsWindow view and its model.
        ///
        /// Pauses the main window until closed.
        ///
        /// Implements UpdateSavedFiles to track files with tags/notes and
        /// ensures cancelled edits do not affect saved files.
        /// </remarks>
        private void ShowFileDetailsPopup(File file)
        {
            if (file == null) return;

            var detailsWindow = new FileDetailsWindow(file, _aiService, _globalTags.ToList());

            bool? result = detailsWindow.ShowDialog();
            if (result == true) { UpdateSavedFiles(file); }

            //detailsWindow.ShowDialog();
        }

        /// <summary>
        /// Method to pupulate File's fields as well as the SavedFiles collection.
        /// </summary>
        /// <param name="file">The file that suffers changes</param>
        /// <variable SavedFiles> </variable>
        /// <variable _globalTags> </variable>
        /// <remarks>
        /// 
        /// We only populate the variables IF they have tags or notes, otherwise we automatically remove them from the SavedFiles collection.
        /// </remarks>
        private void UpdateSavedFiles(File file)
        {
            bool hasTags = file.Relic.Tags != null && file.Relic.Tags.Count > 0;
            bool hasNotes = !string.IsNullOrWhiteSpace(file.Relic.Notes);

            if (hasTags || hasNotes)
            {
                if (!SavedFiles.Contains(file))
                {
                    SavedFiles.Add(file);
                }

                if(hasTags)
                {
                    foreach (var tag in file.Relic.Tags)
                    {
                        _globalTags.Add(tag);
                    }   
                }
            }
            else
            {
                if (SavedFiles.Contains(file))
                {
                    SavedFiles.Remove(file);
                }
            }
        }

        /// <summary>
        /// Method to execute when a file is double-clicked in the UI.
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteOpenFile(object parameter)
        {
            if (parameter is File file)
            {
                ShowFileDetailsPopup(file);
            }
        }

        private void ExecuteAddTag(object parameter)
        {
            var selectedFiles = InputFiles.Where(f => f.IsSelected).ToList();

            if (selectedFiles.Count == 0) return;
            var tagView = new FileTagAdderView(selectedFiles, _globalTags);

            tagView.Owner = Application.Current.MainWindow; 
            tagView.ShowDialog();

            foreach (var file in selectedFiles)
            {
                UpdateSavedFiles(file);
            }
        }

        private void PopOutSearch()
        {
            var searchView = new SearchPopOutView(SavedFiles, _globalTags);

            bool? result = searchView.ShowDialog();

            if (result == true)
            {
                if (searchView.DataContext is SearchPopOutViewModel childvm)
                {
                    try
                    {
                        PopulatedFiles = new ObservableCollection<File>(childvm.SortedFiles);
                    }

                    catch(Exception ex) 
                    {
                        PopulatedFiles = null;
                    }
                }
            }
        }
    }
}
