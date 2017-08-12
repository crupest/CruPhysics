using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CruPhysics.Controls;
using CruPhysics.Shapes.SelectionBox;

namespace CruPhysics.PhysicalObjects.Views
{
    public partial class ControllerView : UserControl
    {
        public ControllerView()
        {
            InitializeComponent();
        }

        public Controller ViewModel => (Controller) DataContext;

        private WorldCanvas Canvas => (WorldCanvas) Parent;

        private Vector delta;

        private Point GetMousePosition(MouseEventArgs args)
        {
            return Canvas.TransformPoint(args.GetPosition(Canvas));
        }

        private void Shape_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            delta = GetMousePosition(e) - ViewModel.Position;
            ((IInputElement) sender).CaptureMouse();
            e.Handled = true;
        }

        private void Shape_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((IInputElement) sender).ReleaseMouseCapture();
            e.Handled = true;
        }

        private void Shape_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (((IInputElement)sender).IsMouseCaptured)
                ViewModel.OnMove(GetMousePosition(e) - delta);
            e.Handled = true;
        }
    }
}
