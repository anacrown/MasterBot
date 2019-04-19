using System.Windows;
using System.Windows.Controls;

namespace BattleBot_SimpleAI.Controls
{
    /// <summary>
    /// Логика взаимодействия для BattleSolverControl.xaml
    /// </summary>
    public partial class BattleSolverControl : UserControl
    {
        public static readonly DependencyProperty BattleSolverProperty = DependencyProperty.Register(
            "BattleSolver", typeof(BattleSolver.BattleSolverSimple), typeof(BattleSolverControl), new PropertyMetadata(default(BattleSolver.BattleSolverSimple)));

        public BattleSolver.BattleSolverSimple BattleSolver
        {
            get => (BattleSolver.BattleSolverSimple) GetValue(BattleSolverProperty);
            set => SetValue(BattleSolverProperty, value);
        }

        public BattleSolverControl(BattleSolver.BattleSolverSimple battleSolver)
        {
            InitializeComponent();
        }
    }
}
