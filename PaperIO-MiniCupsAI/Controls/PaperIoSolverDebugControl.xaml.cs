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

        private System.Windows.Controls.Image[,] _images;
        private Label[,] _labelsMe;
        private Label[,] _labelsOpp;
        private Label[,] _labelsRev;
        private System.Drawing.Size _size;

        public PaperIoSolver Solver
        {
            get { return (PaperIoSolver) this.GetValue(PaperIoSolverDebugControl.SolverProperty); }
            set { this.SetValue(PaperIoSolverDebugControl.SolverProperty, value); }
        }

        public PaperIoSolverDebugControl(PaperIoSolver solver) : this()
        {
            this.Solver = solver;
            this.Solver.BoardChanged += (EventHandler<PaperIO_MiniCupsAI.Board>) ((sender, board) =>
                this.Dispatcher.InvokeAsync(() => this.UpdateView(board)));
        }

        private void UpdateView(PaperIO_MiniCupsAI.Board board)
        {
            if (this._size.IsEmpty)
            {
                this._size = board.Size;
                this._images = new System.Windows.Controls.Image[this._size.Width, this._size.Height];
                this._labelsMe = new Label[this._size.Width, this._size.Height];
                this._labelsOpp = new Label[this._size.Width, this._size.Height];
                this._labelsRev = new Label[this._size.Width, this._size.Height];
                int width = Properties.Resources.none.Width;
                int height = Properties.Resources.none.Height;
                this.Canvas.Width = this._size.Width * width;
                this.Canvas.Height = this._size.Height * height;
                for (int index1 = 0; index1 < this._size.Width; ++index1)
                {
                    for (int index2 = 0; index2 < this._size.Height; ++index2)
                    {
                        System.Windows.Controls.Image[,] images = this._images;
                        int index3 = index1;
                        int index4 = index2;
                        System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                        image.Width = width;
                        image.Height = height;
                        image.SnapsToDevicePixels = true;
                        images[index3, index4] = image;
                        this.Canvas.Children.Add(this._images[index1, index2]);
                        Canvas.SetLeft(this._images[index1, index2], index1 * width);
                        Canvas.SetBottom(this._images[index1, index2], index2 * height);
                        Label[,] labelsMe = this._labelsMe;
                        int index5 = index1;
                        int index6 = index2;
                        Label label1 = new Label();
                        label1.FontSize = 10.0;
                        label1.Foreground = System.Windows.Media.Brushes.Green;
                        label1.SnapsToDevicePixels = true;
                        labelsMe[index5, index6] = label1;
                        this.Canvas.Children.Add(this._labelsMe[index1, index2]);
                        Canvas.SetLeft(this._labelsMe[index1, index2], index1 * width);
                        Canvas.SetBottom(this._labelsMe[index1, index2], index2 * height + 12);
                        Label[,] labelsOpp = this._labelsOpp;
                        int index7 = index1;
                        int index8 = index2;
                        Label label2 = new Label();
                        label2.FontSize = 10.0;
                        label2.Foreground = System.Windows.Media.Brushes.Black;
                        label2.SnapsToDevicePixels = true;
                        labelsOpp[index7, index8] = label2;
                        this.Canvas.Children.Add(this._labelsOpp[index1, index2]);
                        Canvas.SetLeft(this._labelsOpp[index1, index2], index1 * width);
                        Canvas.SetBottom(this._labelsOpp[index1, index2], index2 * height + 4);
                    }
                }
            }

            for (int i = 0; i < this._size.Width; i++)
            {
                for (int j = 0; j < this._size.Height; j++)
                {
                    PaperIO_MiniCupsAI.Cell cell = board[i, j];
                    Element element = board.MeCell == null || !(board.MeCell.Pos == cell.Pos)
                        ? cell.Element
                        : Element.ME;
                    this._images[i, j].Source = ResourceManager.GetSource(element);
                    if (board.MeWeight != null)
                    {
                        this._labelsMe[i, j].Content = board.MeWeight[i, j].Weight;
                        this._labelsOpp[i, j].Content = board.OppWeights.Values
                            .Select(map => map[i, j].Weight).Min();
                    }
                }
            }
        }
    }
}
