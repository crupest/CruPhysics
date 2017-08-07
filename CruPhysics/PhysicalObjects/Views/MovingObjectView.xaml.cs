using System.Windows.Controls;
using System.Windows.Input;

namespace CruPhysics.PhysicalObjects.Views
{
    public partial class MovingObjectView : UserControl
    {
        public MovingObjectView()
        {
            InitializeComponent();
        }

        public MovingObject ViewModel => (MovingObject) DataContext;

        private void Shape_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ViewModel.OnMouseEnter();
        }

        private void Shape_OnMouseLeave(object sender, MouseEventArgs e)
        {
            ViewModel.OnMouseLeave();
        }
    }
}
