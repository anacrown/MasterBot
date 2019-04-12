using System.Windows;
using System.Windows.Controls;
using BattleBot_SuperAI.BattleSolver;
using CodenjoyBot.Board;

namespace BattleBot_SuperAI.Controls
{
    public partial class BattleSolverDebugControl : UserControl
    {
        private int _size;
        private Image[,] _images;

        public static readonly DependencyProperty BattleSolverProperty = DependencyProperty.Register(
            "BattleSolver", typeof(BattleSolver.BattleSolver), typeof(BattleSolverDebugControl), new PropertyMetadata(default(BattleSolver.BattleSolver)));

        public BattleSolver.BattleSolver BattleSolver
        {
            get { return (BattleSolver.BattleSolver)GetValue(BattleSolverProperty); }
            set { SetValue(BattleSolverProperty, value); }
        }

        public static readonly DependencyProperty IsDrawProperty = DependencyProperty.Register(
            "IsDraw", typeof(bool?), typeof(BattleSolverDebugControl), new PropertyMetadata(default(bool?)));

        public bool? IsDraw
        {
            get => (bool?)GetValue(IsDrawProperty);
            set => SetValue(IsDrawProperty, value);
        }

        public BattleSolverDebugControl(BattleSolver.BattleSolver battleSolver)
        {
            InitializeComponent();

            IsDraw = true;

            BattleSolver = battleSolver;
            BattleSolver.BoardChanged += (sender, board) =>
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (IsDraw.Value)
                        UpdateView(board);
                });
            };
        }

        private void UpdateView(Board board)
        {
            if (_size == 0)
            {
                _size = board.Size;
                _images = new Image[_size, _size];

                var offsetX = Properties.Resources.none.Width;
                var offsetY = Properties.Resources.none.Height;

                Canvas.Width = _size * offsetX;
                Canvas.Height = _size * offsetY;

                for (var i = 0; i < _size; i++)
                {
                    for (var j = 0; j < _size; j++)
                    {
                        _images[i, j] = new Image()
                        {
                            Width = offsetX,
                            Height = offsetY,
                            SnapsToDevicePixels = true
                        };

                        Canvas.Children.Add(_images[i, j]);
                        Canvas.SetLeft(_images[i, j], i * offsetY);
                        Canvas.SetTop(_images[i, j], j * offsetX);
                    }
                }
            }

            for (var i = 0; i < _size; i++)
            {
                for (var j = 0; j < _size; j++)
                {
                    _images[i, j].Source = ResourceManager.GetSource(board[i, j].GetElement());
                }
            }
        }
    }
}
