using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodenjoyBot.Board;
using Image = System.Windows.Controls.Image;

namespace BomberMan_SuperAI
{
    /// <summary>
    /// Interaction logic for BomberSolverControl.xaml
    /// </summary>
    public partial class BomberSolverControl
    {
        private int Size;
        private Image[,] Images;
        public BomberSolver BomberSolver { get; }

        public BomberSolverControl(BomberSolver bomberSolver)
        {
            BomberSolver = bomberSolver;
            BomberSolver.BoardChanged += BomberSolverOnBoardChanged;
            InitializeComponent();
        }

        private void BomberSolverOnBoardChanged(object sender, Board board)
        {
            Dispatcher.InvokeAsync(() => UpdateView(board));
        }

        private void UpdateView(Board board)
        {
            try
            {
                if (Size == 0)
                {
                    Size = board.Size;
                    Images = new Image[Size, Size];

                    var offsetX = Properties.Resources.bomb_bomberman.Width;
                    var offsetY = Properties.Resources.bomb_bomberman.Height;

                    for (int i = 0; i < Size; i++)
                    {
                        for (int j = 0; j < Size; j++)
                        {
                            Images[i, j] = new Image();
                            Canvas.Children.Add(Images[i, j]);
                            Canvas.SetTop(Images[i, j], i * offsetX);
                            Canvas.SetLeft(Images[i, j], j * offsetY);
                        }
                    }
                }

                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        Images[i, j].Source = GetSource(board[i, j].GetElement());
                    }
                }
            }
            catch (Exception ex)
            {
                var a = 0;
            }
        }

        private ImageSource GetSource(Element element)
        {
            switch (element)
            {
                case Element.BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.bomberman);
                case Element.BOMB_BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.bomb_bomberman);
                case Element.DEAD_BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.dead_bomberman);
                case Element.OTHER_BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.other_bomberman);
                case Element.OTHER_BOMB_BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.other_bomb_bomberman);
                case Element.OTHER_DEAD_BOMBERMAN:
                    return SourceFromBitmap(Properties.Resources.other_dead_bomberman);
                case Element.BOMB_TIMER_5:
                    return SourceFromBitmap(Properties.Resources.bomb_timer_5);
                case Element.BOMB_TIMER_4:
                    return SourceFromBitmap(Properties.Resources.bomb_timer_4);
                case Element.BOMB_TIMER_3:
                    return SourceFromBitmap(Properties.Resources.bomb_timer_3);
                case Element.BOMB_TIMER_2:
                    return SourceFromBitmap(Properties.Resources.bomb_timer_2);
                case Element.BOMB_TIMER_1:
                    return SourceFromBitmap(Properties.Resources.bomb_timer_1);
                case Element.BOOM:
                    return SourceFromBitmap(Properties.Resources.boom);
                case Element.WALL:
                    return SourceFromBitmap(Properties.Resources.wall);
                case Element.DESTROYABLE_WALL:
                    return SourceFromBitmap(Properties.Resources.destroyable_wall);
                case Element.DESTROYED_WALL:
                    return SourceFromBitmap(Properties.Resources.destroyed_wall);
                case Element.MEAT_CHOPPER:
                    return SourceFromBitmap(Properties.Resources.meat_chopper);
                case Element.DEAD_MEAT_CHOPPER:
                    return SourceFromBitmap(Properties.Resources.dead_meat_chopper);
                case Element.NONE:
                    return SourceFromBitmap(Properties.Resources.none);
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }

        public BitmapImage SourceFromBitmap(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
