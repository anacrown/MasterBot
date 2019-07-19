using System.Windows;
using System.Windows.Controls;

namespace PaperIO_MiniCupsAI.Controls
{
    /// <summary>
    /// Логика взаимодействия для PaperIoSolverControl.xaml
    /// </summary>
    public partial class PaperIoSolverControl : UserControl
    {
        public static readonly DependencyProperty SolverProperty = DependencyProperty.Register(
            "Solver", typeof(PaperIoSolver), typeof(PaperIoSolverControl), new PropertyMetadata(default(PaperIoSolver)));

        public PaperIoSolver Solver
        {
            get => (PaperIoSolver) GetValue(SolverProperty);
            set => SetValue(SolverProperty, value);
        }
        public PaperIoSolverControl()
        {
            InitializeComponent();
        }

        public PaperIoSolverControl(PaperIoSolver solver) : this()
        {
            Solver = solver;
        }
    }
}
