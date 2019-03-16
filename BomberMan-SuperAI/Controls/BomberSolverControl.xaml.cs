using System.Windows;
using System.Windows.Controls;

namespace BomberMan_SuperAI.Controls
{
    /// <summary>
    /// Interaction logic for BomberSolverControl.xaml
    /// </summary>
    public partial class BomberSolverControl : UserControl
    {
        public static readonly DependencyProperty BomberSolverProperty = DependencyProperty.Register(
            "BomberSolver", typeof(BomberSolver), typeof(BomberSolverControl), new PropertyMetadata(default(BomberSolver)));

        public BomberSolver BomberSolver
        {
            get => (BomberSolver) GetValue(BomberSolverProperty);
            set => SetValue(BomberSolverProperty, value);
        }

        public BomberSolverControl()
        {
            InitializeComponent();
        }

        public BomberSolverControl(BomberSolver bomberSolver) : this()
        {
            BomberSolver = bomberSolver;
        }
    }
}
