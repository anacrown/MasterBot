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
