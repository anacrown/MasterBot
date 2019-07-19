using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PaperIO_MiniCupsAI
{
    static class ResourceManager
    {
        private static Dictionary<Element, BitmapImage> _bitmapImages = new Dictionary<Element, BitmapImage>();

        public static ImageSource GetSource(Element element)
        {
            if (!_bitmapImages.ContainsKey(element))
            {
                BitmapImage bitmapImage;
                switch (element)
                {
                    case Element.ME:
                        bitmapImage = SourceFromBitmap(Properties.Resources.me);
                        break;
                    case Element.ME_LINE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.me_line);
                        break;
                    case Element.ME_TERRITORY:
                        bitmapImage = SourceFromBitmap(Properties.Resources.me_territory);
                        break;
                    case Element.PLAYER:
                        bitmapImage = SourceFromBitmap(Properties.Resources.player);
                        break;
                    case Element.PLAYER_LINE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.player_line);
                        break;
                    case Element.PLAYER_TERRITORY:
                        bitmapImage = SourceFromBitmap(Properties.Resources.player_territory);
                        break;
                    case Element.NONE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.none);
                        break;
                    case Element.EXPLORER:
                        bitmapImage = SourceFromBitmap(Properties.Resources.explorer);
                        break;
                    case Element.FLASH:
                        bitmapImage = SourceFromBitmap(Properties.Resources.flash);
                        break;
                    case Element.SAW:
                        bitmapImage = SourceFromBitmap(Properties.Resources.saw);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(element), element, null);
                }

                _bitmapImages.Add(element, bitmapImage);
            }

            return _bitmapImages[element];
        }

        private static BitmapImage SourceFromBitmap(Bitmap src)
        {
            var ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
