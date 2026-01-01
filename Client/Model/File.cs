using AplicationUI.Model;
using FileExplorer.Model;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Media;

public enum  FileType
{
    Unknown,Image,Audio,Video,TextFile,Document, Folder //Don't forget to implement Folder
}

public class File
{
    public string Name { get; protected set; } = string.Empty;
    public ImageSource Icon { get; protected set; } 
    protected string Path { get; set; } = string.Empty;
    protected string Extension { get; set; }
    protected FileType Type { get; set; }

    //

    protected IFilePreview? Preview { get; set; }

    public FileReliquary Relic { get; set; }
    public FileMetaData? Metadata { get; set; }
    

    public File()
    {
        Extension= string.Empty;   
    }

    public File(string name, ImageSource icon, string path)
    {
        this.Path = path;
        this.Name = name;
        this.Icon = icon;
        Extension = System.IO.Path.GetExtension(path);
        Type = ManageFileType(Extension);
        Preview = HandlePreview();

        Relic = new FileReliquary();
        Metadata = null;        //It takes extra space when doing a short preview
    }

    public override string ToString()
    {
        return Name;
    }

    public string GetPath()
    {
        return Path;
    }

    public string GetFileType()
    {
        return Type.ToString();
    }


    public FileType ManageFileType(string extension)
    {
        extension = extension.ToLowerInvariant();

        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
            case ".png": 
                    return FileType.Image;

            case ".mp3":
            case ".wav":
            case ".flacc":
                return FileType.Audio;

            case ".mp4":
            case ".avi":
                return FileType.Video;

            case ".pdf":
            case ".doc":
            case ".docx":
                return FileType.Document;

            default:
                return FileType.Unknown;
        }
    }

    /// <summary>
    /// Generates a different file preview based on the current file type.
    /// </summary>
    /// <returns></returns>
    private IFilePreview? HandlePreview()
    {
        switch (Type)
        {
            case FileType.Image:
                {
                    //Icon = new System.Windows.Media.Imaging.BitmapImage(new Uri(Path));

                    var preview = new ImagePreview(Path);
                    Icon = preview.getActualIcon();

                    return preview;
                }
            default:
                return null;
        }
    }

}
