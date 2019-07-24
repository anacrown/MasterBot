using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PaperIO_MiniCupsAI.DataContract;

namespace PaperIO_MiniCupsAI.Controls
{
    /// <summary>
    /// Interaction logic for PaperIoSolverDebugControl.xaml
    /// </summary>
    public partial class PaperIoSolverDebugControl : UserControl
    {
        public PaperIoSolverDebugControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SolverProperty = DependencyProperty.Register(nameof(Solver),
            typeof(PaperIoSolver), typeof(PaperIoSolverDebugControl), new PropertyMetadata((object) null));

        private Image[,] _images;
        private Label[,] _labelsMe;
        private Label[,] _labelsOpp;
        private Label[,] _labelsRev;
        private System.Drawing.Size _size;

        public PaperIoSolver Solver
        {
            get => (PaperIoSolver) GetValue(SolverProperty);
            set => SetValue(SolverProperty, value);
        }

        public PaperIoSolverDebugControl(PaperIoSolver solver) : this()
        {
            Solver = solver;
            Solver.BoardChanged += (EventHandler<Board>) ((sender, board) =>
                Dispatcher.InvokeAsync(() => UpdateView(board)));
        }

        private void UpdateView(Board board)
        {
            if (_size.IsEmpty)
            {
                _size = board.Size;

                _images = new Image[_size.Width, _size.Height];
                _labelsMe = new Label[_size.Width, _size.Height];
                _labelsOpp = new Label[_size.Width, _size.Height];
                _labelsRev = new Label[_size.Width, _size.Height];

                var width = board.JPacket.Params.Width;
                var height = board.JPacket.Params.Width;

                Canvas.Width = _size.Width * width;
                Canvas.Height = _size.Height * height;

                for (var i = 0; i < _size.Width; ++i)
                {
                    for (var j = 0; j < _size.Height; ++j)
                    {
                        _images[i, j] = new Image {Width = width, Height = height, SnapsToDevicePixels = true};


                        Canvas.Children.Add(_images[i, j]);
                        Canvas.SetLeft(_images[i, j], i * width);
                        Canvas.SetBottom(_images[i, j], j * height);

                        _labelsMe[i, j] = new Label
                        {
                            FontSize = 10.0, Foreground = Brushes.Green, SnapsToDevicePixels = true
                        };

                        Canvas.Children.Add(_labelsMe[i, j]);
                        Canvas.SetLeft(_labelsMe[i, j], i * width);
                        Canvas.SetBottom(_labelsMe[i, j], j * height + 12);

                        _labelsOpp[i, j] = new Label
                        {
                            FontSize = 10.0, Foreground = Brushes.Black, SnapsToDevicePixels = true
                        };

                        Canvas.Children.Add(_labelsOpp[i, j]);
                        Canvas.SetLeft(_labelsOpp[i, j], i * width);
                        Canvas.SetBottom(_labelsOpp[i, j], j * height + 4);
                    }
                }
            }

            for (var i = 0; i < _size.Width; i++)
            {
                for (var j = 0; j < _size.Height; j++)
                {
                    _images[i, j].Source = ResourceManager.GetSource(board[i,j].Element);
                    if (board.JPacket.PacketType == JPacketType.Tick)
                    {
                        _labelsMe[i, j].Content = board.IPlayer.Map[i, j].Weight;
                        _labelsOpp[i, j].Content = board.Enemies.Select(enemy => enemy.Map[i, j].Weight).Min();
                    }
                }
            }
        }
    }
}
