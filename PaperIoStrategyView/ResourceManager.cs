using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PaperIoStrategy.AISolver;
using PaperIoStrategyView.Properties;

namespace PaperIoStrategyView
{
    internal static class ResourceManager
    {
        private static Dictionary<Element, BitmapImage> _bitmapImages = new Dictionary<Element, BitmapImage>();
        private static Dictionary<string, BitmapImage> _bitmapImagesSome = new Dictionary<string, BitmapImage>();

        public static ImageSource GetSource(string name)
        {
            if (!_bitmapImagesSome.ContainsKey(name))
                _bitmapImagesSome.Add(name, SourceFromBitmap(Resources.ResourceManager.GetObject(name) as Bitmap));
            return _bitmapImagesSome[name];
        }

        public static ImageSource GetSource(Element element)
        {
            if (!_bitmapImages.ContainsKey(element))
            {
                BitmapImage bitmapImage;
                switch (element)
                {
                    case Element.ME:
                        bitmapImage = SourceFromBitmap(Resources.me);
                        break;
                    case Element.ME_LINE:
                        bitmapImage = SourceFromBitmap(Resources.me_line);
                        break;
                    case Element.ME_TERRITORY:
                        bitmapImage = SourceFromBitmap(Resources.me_territory);
                        break;
                    case Element.PLAYER:
                        bitmapImage = SourceFromBitmap(Resources.player);
                        break;
                    case Element.PLAYER_LINE:
                        bitmapImage = SourceFromBitmap(Resources.player_line);
                        break;
                    case Element.PLAYER_TERRITORY:
                        bitmapImage = SourceFromBitmap(Resources.player_territory);
                        break;
                    case Element.NONE:
                        bitmapImage = SourceFromBitmap(Resources.none);
                        break;
                    case Element.EXPLORER:
                        bitmapImage = SourceFromBitmap(Resources.explorer);
                        break;
                    case Element.FLASH:
                        bitmapImage = SourceFromBitmap(Resources.flash);
                        break;
                    case Element.SAW:
                        bitmapImage = SourceFromBitmap(Resources.saw);
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
            var memoryStream = new MemoryStream();
            src.Save(memoryStream, ImageFormat.Bmp);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            memoryStream.Seek(0L, SeekOrigin.Begin);
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }
}
