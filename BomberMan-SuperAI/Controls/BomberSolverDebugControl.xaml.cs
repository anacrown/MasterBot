using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.Board;

namespace BomberMan_SuperAI.Controls
{
    public partial class BomberSolverDebugControl : INotifyPropertyChanged
    {
        private int _size;
        private Image[,] _images;
        private bool? _isDraw;

        public static readonly DependencyProperty BomberSolverProperty = DependencyProperty.Register(
            "BomberSolver", typeof(BomberSolver), typeof(BomberSolverDebugControl), new PropertyMetadata(default(BomberSolver)));

        public BomberSolver BomberSolver
        {
            get => (BomberSolver) GetValue(BomberSolverProperty);
            set => SetValue(BomberSolverProperty, value);
        }

        public bool? IsDraw
        {
            get => _isDraw;
            set
            {
                if (value == _isDraw) return;
                _isDraw = value;
                OnPropertyChanged();
            }
        }

        public BomberSolverDebugControl(BomberSolver bomberSolver)
        {
            IsDraw = true;

            InitializeComponent();

            BomberSolver = bomberSolver;
            BomberSolver.BoardChanged += (sender, board) =>
            {
                if (IsDraw.HasValue && IsDraw.Value)
                    Dispatcher.InvokeAsync(() => UpdateView(board));
            };
        }

        private void UpdateView(Board board)
        {
            if (_size == 0)
            {
                _size = board.Size;
                _images = new Image[_size, _size];

                var offsetX = Properties.Resources.bomb_bomberman.Width;
                var offsetY = Properties.Resources.bomb_bomberman.Height;

                Canvas.Width = _size * offsetX;
                Canvas.Height = _size * offsetY;

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
