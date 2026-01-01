using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModel
{
    public interface IFileDetailsInterface
    {
    }

    public class DefaultPreview : IFileDetailsInterface
    {
        public File FileData { get; }
        public string Path;

        public DefaultPreview(File file)
        {
            FileData = file;
            Path = file.GetPath();
        }
    }


    public class ImagePreview : IFileDetailsInterface
    {
        public File FileData { get; }
        public string Path { get; }

        public ImagePreview(File file)
        {
            FileData = file;
            Path = file.GetPath();
        }
    }
}
