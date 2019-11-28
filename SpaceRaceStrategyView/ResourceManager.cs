using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpaceRaceStrategy;
using SpaceRaceStrategyView.Properties;

namespace SpaceRaceStrategyView
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
            if (_bitmapImages.ContainsKey(element)) return _bitmapImages[element];

            BitmapImage bitmapImage;
            switch (element)
            {
                case Element.NONE:
                    bitmapImage = SourceFromBitmap(Resources.none);
                    break;
                case Element.EXPLOSION:
                    bitmapImage = SourceFromBitmap(Resources.explosion);
                    break;
                case Element.WALL:
                    bitmapImage = SourceFromBitmap(Resources.wall);
                    break;
                case Element.HERO:
                    bitmapImage = SourceFromBitmap(Resources.hero);
                    break;
                case Element.OTHER_HERO:
                    bitmapImage = SourceFromBitmap(Resources.other_hero);
                    break;
                case Element.DEAD_HERO:
                    bitmapImage = SourceFromBitmap(Resources.dead_hero);
                    break;
                case Element.GOLD:
                    bitmapImage = SourceFromBitmap(Resources.gold);
                    break;
                case Element.BOMB:
                    bitmapImage = SourceFromBitmap(Resources.bomb);
                    break;
                case Element.STONE:
                    bitmapImage = SourceFromBitmap(Resources.stone);
                    break;
                case Element.BULLET_PACK:
                    bitmapImage = SourceFromBitmap(Resources.bullet_pack);
                    break;
                case Element.BULLET:
                    bitmapImage = SourceFromBitmap(Resources.bullet);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }

            _bitmapImages.Add(element, bitmapImage);
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
