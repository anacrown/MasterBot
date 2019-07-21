using System.Windows;
using System.Windows.Controls;
using BattleBot_SimpleAI.BattleSolver;
using CodenjoyBot.Board;

namespace BattleBot_SimpleAI.Controls
{
    public partial class BattleSolverDebugControl : UserControl
    {
        private int _size;
        private Image[,] _images;

        public static readonly DependencyProperty BattleSolverProperty = DependencyProperty.Register(
            "BattleSolver", typeof(BattleSolver.BattleSolverSimple), typeof(BattleSolverDebugControl), new PropertyMetadata(default(BattleSolver.BattleSolverSimple)));

        public BattleSolver.BattleSolverSimple BattleSolver
        {
            get { return (BattleSolver.BattleSolverSimple)GetValue(BattleSolverProperty); }
            set { SetValue(BattleSolverProperty, value); }
        }

        public static readonly DependencyProperty IsDrawProperty = DependencyProperty.Register(
            "IsDraw", typeof(bool?), typeof(BattleSolverDebugControl), new PropertyMetadata(default(bool?)));

        public bool? IsDraw
        {
            get => (bool?)GetValue(IsDrawProperty);
            set => SetValue(IsDrawProperty, value);
        }

        public BattleSolverDebugControl(BattleSolver.BattleSolverSimple battleSolver)
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

        private void UpdateView(Board<Cell> board)
        {
            if (_size == 0)
            {
                _size = board.Size.Width;
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
