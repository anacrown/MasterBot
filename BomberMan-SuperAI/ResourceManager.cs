using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BomberMan_SuperAI.BattleSolver;

namespace BomberMan_SuperAI
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
                    case Element.BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomberman);
                        break;
                    case Element.BOMB_BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_bomberman);
                        break;
                    case Element.DEAD_BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.dead_bomberman);
                        break;
                    case Element.OTHER_BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_bomberman);
                        break;
                    case Element.OTHER_BOMB_BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_bomb_bomberman);
                        break;
                    case Element.OTHER_DEAD_BOMBERMAN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_dead_bomberman);
                        break;
                    case Element.BOMB_TIMER_5:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_timer_5);
                        break;
                    case Element.BOMB_TIMER_4:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_timer_4);
                        break;
                    case Element.BOMB_TIMER_3:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_timer_3);
                        break;
                    case Element.BOMB_TIMER_2:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_timer_2);
                        break;
                    case Element.BOMB_TIMER_1:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bomb_timer_1);
                        break;
                    case Element.BOOM:
                        bitmapImage = SourceFromBitmap(Properties.Resources.boom);
                        break;
                    case Element.WALL:
                        bitmapImage = SourceFromBitmap(Properties.Resources.wall);
                        break;
                    case Element.DESTROYABLE_WALL:
                        bitmapImage = SourceFromBitmap(Properties.Resources.destroyable_wall);
                        break;
                    case Element.DESTROYED_WALL:
                        bitmapImage = SourceFromBitmap(Properties.Resources.destroyed_wall);
                        break;
                    case Element.MEAT_CHOPPER:
                        bitmapImage = SourceFromBitmap(Properties.Resources.meat_chopper);
                        break;
                    case Element.DEAD_MEAT_CHOPPER:
                        bitmapImage = SourceFromBitmap(Properties.Resources.dead_meat_chopper);
                        break;
                    case Element.NONE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.none);
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