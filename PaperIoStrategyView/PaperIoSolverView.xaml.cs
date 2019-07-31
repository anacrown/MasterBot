using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PaperIoStrategy;
using PaperIoStrategy.AISolver;
using PaperIoStrategy.DataContract;

namespace PaperIoStrategyView
{
    /// <summary>
    /// Логика взаимодействия для PaperIoSolverView.xaml
    /// </summary>
    public partial class PaperIoSolverView : UserControl
    {
        public static readonly DependencyProperty SolverProperty = DependencyProperty.Register(
            "Solver", typeof(PaperIoSolver), typeof(PaperIoSolverView), new PropertyMetadata(default(PaperIoSolver)));

        public PaperIoSolver Solver
        {
            get => (PaperIoSolver)GetValue(SolverProperty);
            set => SetValue(SolverProperty, value);
        }

        private Image[,] _images;
        private Label[,] _labels;
//        private Label[,] _labelsOpp;
//        private Label[,] _labelsRev;

        public PaperIoSolverView()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == SolverProperty)
            {
                if (e.OldValue is PaperIoSolver oldSolver)
                    oldSolver.BoardChanged -= SolverOnBoardChanged;

                if (e.NewValue is PaperIoSolver newSolver)
                    newSolver.BoardChanged += SolverOnBoardChanged;
            }
        }

        private void SolverOnBoardChanged(object sender, Board board) => Dispatcher.InvokeAsync(() => UpdateView(board));

        private void UpdateView(Board board)
        {
            if (board.JPacket.PacketType == JPacketType.StartGame)
            {
                _images = new Image[board.Size.Width, board.Size.Height];
                _labels = new Label[board.Size.Width, board.Size.Height];
//                _labelsOpp = new Label[board.Size.Width, board.Size.Height];
//                _labelsRev = new Label[board.Size.Width, board.Size.Height];

                var width = board.JPacket.Params.Width;
                var height = board.JPacket.Params.Width;

                Canvas.Width = board.Size.Width * width;
                Canvas.Height = board.Size.Height * height;

                for (var i = 0; i < board.Size.Width; ++i)
                {
                    for (var j = 0; j < board.Size.Height; ++j)
                    {
                        _images[i, j] = new Image
                        {
                            Width = width,
                            Height = height,
                            SnapsToDevicePixels = true
                        };


                        Canvas.Children.Add(_images[i, j]);
                        Canvas.SetLeft(_images[i, j], i * width);
                        Canvas.SetBottom(_images[i, j], j * height);
                    }
                }

                for (var i = 0; i < board.Size.Width; ++i)
                {
                    for (var j = 0; j < board.Size.Height; ++j)
                    {
                        _labels[i, j] = new Label
                        {
                            FontSize = 10.0,
                            Foreground = Brushes.Green,
                            SnapsToDevicePixels = true
                        };

                        Canvas.Children.Add(_labels[i, j]);
                        Canvas.SetLeft(_labels[i, j], i * width);
                        Canvas.SetBottom(_labels[i, j], j * height + 12);
                    }
                }
            }

            for (var i = 0; i < board.Size.Width; i++)
            {
                for (var j = 0; j < board.Size.Height; j++)
                {
                    _labels[i, j].Content = null;
                    if (board[i, j].Element == Element.NONE && board.Paths.Any(path => path.Contains(board[i, j].Pos)))
                    {
                        _images[i, j].Source = ResourceManager.GetSource("path");
                    }
                    else
                    {
                        _images[i, j].Source = ResourceManager.GetSource(board[i, j].Element);
                    }
                    //                    if (board.JPacket.PacketType == JPacketType.Tick)
                    //                    {
                    //                        _labelsMe[i, j].Content = board.IPlayer.Map[i, j].Weight;
                    //
                    //                        if (board.Enemies.Any())
                    //                            _labelsOpp[i, j].Content = board.EnemiesMap(i, j);
                    //                    }
                }
            }

            if (board.Players != null && board.Players.Any())
            {
                foreach (var player in board.Players)
                {
                    _labels[player.Value.Position.X, player.Value.Position.Y].Content = player.Value.JPlayer.Position;
                }
            }
        }
    }
}
