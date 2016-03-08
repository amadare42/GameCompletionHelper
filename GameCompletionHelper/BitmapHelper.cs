using GameCompletionHelper.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameCompletionHelper
{
    public static class BitmapHelper
    {
        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private static BitmapImage noIconImage;

        public static BitmapImage NoIconImage
        {
            get
            {
                if (noIconImage == null)
                {
                    var noIcon = Resources.NoIcon;
                    noIcon.MakeTransparent(noIcon.GetPixel(0, 0));
                    noIconImage = BitmapToImageSource(noIcon);
                }
                return noIconImage;
            }
        }
    }
}