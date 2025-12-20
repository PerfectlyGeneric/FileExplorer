using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;

//System.Drawing.Common installed via NuGet


public abstract class PathHandler
{
    List<File> files = new List<File>();

    public PathHandler()
    {
    }

    /// <summary>
    /// Loads file data from the specified directory path and returns a list of file objects.
    /// </summary>
    /// <param name="path">The path to the directory containing the files to load. The directory must exist.</param>
    /// <returns>A list of <see cref="File"/> objects representing the files in the specified directory. The list will be empty
    /// if the directory contains no files.</returns>
    /// <exception cref="DirectoryNotFoundException">Thrown if the specified <paramref name="path"/> does not exist.</exception>
    public static List<File> LoadFileDataGiven(string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: {path}");

        List<File> fileList = new List<File>();
        string[] filePaths = Directory.GetFiles(path);

        foreach (var filePath in filePaths)
        {
            string fileName = Path.GetFileName(filePath);
            ImageSource icon = GetDefaultFileIcon(filePath);
            


            File fileCore = new File(fileName, icon, filePath);
            fileList.Add(fileCore);
        }

        return fileList;
    }

    /// <summary>
    /// Retrieves the default file icon associated with the specified file path.
    /// </summary>
    /// <remarks>This method attempts to extract the default icon associated with the file type of the
    /// specified path. If the file path is invalid or an error occurs during extraction, the method returns <see
    /// langword="null"/>.</remarks>
    /// <param name="filePath">The full path of the file for which to retrieve the default icon.</param>
    /// <returns>An <see cref="ImageSource"/> representing the default icon for the specified file,  or <see langword="null"/> if
    /// the icon cannot be retrieved.</returns>
    public static ImageSource? GetDefaultFileIcon(string filePath)
    {
        try
        {

            var icon = Icon.ExtractAssociatedIcon(filePath);

            if (icon == null)
                return null;   

            return Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
        }
        catch
        {
            return null;
        }
    }
}
