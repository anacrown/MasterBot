using System.Windows.Controls;
using CodenjoyBot.Board;
using Image = System.Windows.Controls.Image;

namespace BomberMan_SuperAI
{
    public partial class BomberSolverControl
    {
        private int _size;
        private Image[,] _images;

        public BomberSolver BomberSolver { get; }

        public BomberSolverControl(BomberSolver bomberSolver)
        {
            InitializeComponent();

            BomberSolver = bomberSolver;
            BomberSolver.BoardChanged += (sender, board) => Dispatcher.InvokeAsync(() => UpdateView(board));
        }

        private void UpdateView(Board board)
        {
            if (_size == 0)
            {
                _size = board.Size;
                _images = new Image[_size, _size];

                var offsetX = Properties.Resources.bomb_bomberman.Width;
                var offsetY = Properties.Resources.bomb_bomberman.Height;

                for (var i = 0; i < _size; i++)
                {
                    for (var j = 0; j < _size; j++)
                    {
                        _images[i, j] = new Image();
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
