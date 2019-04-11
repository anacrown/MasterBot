using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BattleBot_SuperAI.BattleSolver;

namespace BattleBot_SuperAI
{
    static class ResourceManager
    {
        private static readonly Dictionary<Element, BitmapImage> _bitmapImages = new Dictionary<Element, BitmapImage>();

        public static ImageSource GetSource(Element element)
        {
            if (!_bitmapImages.ContainsKey(element))
            {
                BitmapImage bitmapImage;
                switch (element)
                {
                    case Element.GROUND:
                        bitmapImage = SourceFromBitmap(Properties.Resources.none);
                        break;
                    case Element.WALL:
                        bitmapImage = SourceFromBitmap(Properties.Resources.battle_wall);
                        break;
                    case Element.DEAD:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bang);
                        break;
                    case Element.CONSTRUCTION:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_DOWN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_down);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_UP:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_up);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_left);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_right);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_DOWN_TWICE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_down_twice);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_UP_TWICE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_up_twice);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_LEFT_TWICE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_left_twice);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_RIGHT_TWICE:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_right_twice);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_LEFT_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_left_right);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_UP_DOWN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_up_down);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_UP_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_up_left);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_RIGHT_UP:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_right_up);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_DOWN_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_down_left);
                        break;
                    case Element.CONSTRUCTION_DESTROYED_DOWN_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.construction_destroyed_down_right);
                        break;
                    case Element.BULLET:
                        bitmapImage = SourceFromBitmap(Properties.Resources.bullet);
                        break;
                    case Element.TANK_UP:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_tank_up);
                        break;
                    case Element.TANK_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_tank_right);
                        break;
                    case Element.TANK_DOWN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_tank_down);
                        break;
                    case Element.TANK_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.other_tank_left);
                        break;
                    case Element.PLAYER_TANK_UP:
                        bitmapImage = SourceFromBitmap(Properties.Resources.tank_up);
                        break;
                    case Element.PLAYER_TANK_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.tank_right);
                        break;
                    case Element.PLAYER_TANK_DOWN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.tank_down);
                        break;
                    case Element.PLAYER_TANK_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.tank_left);
                        break;
                    case Element.OTHER_TANK_UP:
                        bitmapImage = SourceFromBitmap(Properties.Resources.ai_tank_up);
                        break;
                    case Element.OTHER_TANK_RIGHT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.ai_tank_right);
                        break;
                    case Element.OTHER_TANK_DOWN:
                        bitmapImage = SourceFromBitmap(Properties.Resources.ai_tank_down);
                        break;
                    case Element.OTHER_TANK_LEFT:
                        bitmapImage = SourceFromBitmap(Properties.Resources.ai_tank_left);
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