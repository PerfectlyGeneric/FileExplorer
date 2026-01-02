using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileExplorer.Model
{
    public interface IFilePreview
    {
    }

    public class ImagePreview : IFilePreview
    {
        protected string ImagePath { get; set; }
        protected ImageSource FullImage { get; set; }


        public ImagePreview(string Path)
        {
            ImagePath = Path;
        }

        public void LoadFullImage()
        {
            FullImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(ImagePath));
        }   

        /// <summary>
        /// Retrieves the actual icon image as an <see cref="ImageSource"/>.
        /// </summary>
        /// <remarks>The returned image is loaded from the URI specified by the <c>ImagePath</c> property.
        /// The image is decoded to a width of 64 pixels and is cached for performance.  The returned <see
        /// cref="ImageSource"/> is frozen to make it thread-safe.</remarks>
        /// <returns>An <see cref="ImageSource"/> representing the icon image. The image is preloaded and  optimized for use in
        /// UI elements.</returns>
        public ImageSource getActualIcon()
        {

            BitmapImage image = new BitmapImage();
                image.BeginInit();

            image.DecodePixelWidth = 64;

            image.UriSource = new Uri(ImagePath, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                image.Freeze();
                return image;
        }
    }   
}
