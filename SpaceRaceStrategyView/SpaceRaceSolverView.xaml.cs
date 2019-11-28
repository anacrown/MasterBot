using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BotBase.Board;
using SpaceRaceStrategy;
using SpaceRaceStrategy.AISolver;

namespace SpaceRaceStrategyView
{
    public partial class SpaceRaceSolverView : UserControl
    {
        public static readonly DependencyProperty SolverProperty = DependencyProperty.Register(
            "Solver", typeof(SpaceRaceSolver), typeof(SpaceRaceSolverView), new PropertyMetadata(default(SpaceRaceSolver)));

        public SpaceRaceSolver Solver
        {
            get => (SpaceRaceSolver)GetValue(SolverProperty);
            set => SetValue(SolverProperty, value);
        }
        public SpaceRaceSolverView()
        {
            InitializeComponent();
        }

        private Image[,] _images;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property != SolverProperty) return;

            if (e.OldValue is SpaceRaceSolver oldSolver)
                oldSolver.BoardChanged -= SolverOnBoardChanged;

            if (e.NewValue is SpaceRaceSolver newSolver)
                newSolver.BoardChanged += SolverOnBoardChanged;
        }

        private void SolverOnBoardChanged(object sender, Board board) => Dispatcher.InvokeAsync(() => UpdateView(board));

        private void UpdateView(Board board)
        {
            if (board.Frame.FrameNumber == 0)
            {
                _images = new Image[board.Size.Width, board.Size.Height];

                var width = Properties.Resources.none.Width;
                var height = Properties.Resources.none.Height;

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
            }

            for (var i = 0; i < board.Size.Width; i++)
            {
                for (var j = 0; j < board.Size.Height; j++)
                {
                    _images[i, j].Source = ResourceManager.GetSource(board[i, j].Element);
                }
            }
        }
    }
}
