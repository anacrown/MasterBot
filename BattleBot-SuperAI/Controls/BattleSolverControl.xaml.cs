using System.Windows;
using System.Windows.Controls;

namespace BattleBot_SuperAI.Controls
{
    /// <summary>
    /// Логика взаимодействия для BattleSolverControl.xaml
    /// </summary>
    public partial class BattleSolverControl : UserControl
    {
        public static readonly DependencyProperty BattleSolverProperty = DependencyProperty.Register(
            "BattleSolver", typeof(BattleSolver.BattleSolver), typeof(BattleSolverControl), new PropertyMetadata(default(BattleSolver.BattleSolver)));

        public BattleSolver.BattleSolver BattleSolver
        {
            get { return (BattleSolver.BattleSolver) GetValue(BattleSolverProperty); }
            set { SetValue(BattleSolverProperty, value); }
        }

        public BattleSolverControl(BattleSolver.BattleSolver battleSolver)
        {
            InitializeComponent();
        }
    }
}
