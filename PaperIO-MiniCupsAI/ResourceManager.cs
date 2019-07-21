using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PaperIO_MiniCupsAI.Properties;

namespace PaperIO_MiniCupsAI
{
    internal static class ResourceManager
    {
        private static Dictionary<Element, BitmapImage> _bitmapImages = new Dictionary<Element, BitmapImage>();
        private static Dictionary<string, BitmapImage> _bitmapImagesSome = new Dictionary<string, BitmapImage>();

        public static ImageSource GetSource(string name)
        {
            if (!ResourceManager._bitmapImagesSome.ContainsKey(name))
                ResourceManager._bitmapImagesSome.Add(name, ResourceManager.SourceFromBitmap(Resources.ResourceManager.GetObject(name) as Bitmap));
            return (ImageSource)ResourceManager._bitmapImagesSome[name];
        }

        public static ImageSource GetSource(Element element)
        {
            if (!ResourceManager._bitmapImages.ContainsKey(element))
            {
                BitmapImage bitmapImage;
                switch (element)
                {
                    case Element.ME:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.me);
                        break;
                    case Element.ME_LINE:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.me_line);
                        break;
                    case Element.ME_TERRITORY:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.me_territory);
                        break;
                    case Element.PLAYER:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.player);
                        break;
                    case Element.PLAYER_LINE:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.player_line);
                        break;
                    case Element.PLAYER_TERRITORY:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.player_territory);
                        break;
                    case Element.NONE:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.none);
                        break;
                    case Element.EXPLORER:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.explorer);
                        break;
                    case Element.FLASH:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.flash);
                        break;
                    case Element.SAW:
                        bitmapImage = ResourceManager.SourceFromBitmap(Resources.saw);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(element), (object)element, (string)null);
                }
                ResourceManager._bitmapImages.Add(element, bitmapImage);
            }
            return (ImageSource)ResourceManager._bitmapImages[element];
        }

        private static BitmapImage SourceFromBitmap(Bitmap src)
        {
            MemoryStream memoryStream = new MemoryStream();
            src.Save((Stream)memoryStream, ImageFormat.Bmp);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            memoryStream.Seek(0L, SeekOrigin.Begin);
            bitmapImage.StreamSource = (Stream)memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
