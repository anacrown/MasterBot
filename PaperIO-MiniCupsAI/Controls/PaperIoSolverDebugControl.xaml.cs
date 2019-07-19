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
using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI.Controls
{
    /// <summary>
    /// Логика взаимодействия для PaperIoSolverDebugControl.xaml
    /// </summary>
    public partial class PaperIoSolverDebugControl : UserControl
    {
        private int _size;
        private Image[,] _images;

        public static readonly DependencyProperty SolverProperty = DependencyProperty.Register(
            "Solver", typeof(PaperIoSolver), typeof(PaperIoSolverDebugControl), new PropertyMetadata(default(PaperIoSolver)));

        public PaperIoSolver Solver
        {
            get => (PaperIoSolver) GetValue(SolverProperty);
            set => SetValue(SolverProperty, value);
        }
        public PaperIoSolverDebugControl()
        {
            InitializeComponent();
        }

        public PaperIoSolverDebugControl(PaperIoSolver solver) : this()
        {
            Solver = solver;

            Solver.BoardChanged += (sender, board) => Dispatcher.InvokeAsync(() => UpdateView(board));
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
