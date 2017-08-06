using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using CruPhysics.Controls;
using CruPhysics.Shapes;

namespace CruPhysics.PhysicalObjects.Views
{
    public partial class FieldView : UserControl
    {
        public FieldView()
        {
            InitializeComponent();

            UpdateBinding();

            var descriptor = DependencyPropertyDescriptor.FromProperty(ContentProperty, typeof(ContentControl));
            descriptor.AddValueChanged(ContentControl, (sender, args) => UpdateBinding());
        }

        private void UpdateBinding()
        {
            void ClearBinding()
            {
                BindingOperations.ClearBinding(this, WorldCanvas.LeftProperty);
                BindingOperations.ClearBinding(this, WorldCanvas.TopProperty);
                BindingOperations.ClearBinding(this, WorldCanvas.CenterXProperty);
                BindingOperations.ClearBinding(this, WorldCanvas.CenterYProperty);
            }

            ClearBinding();

            if (ContentControl.Content is CruRectangle)
            {
                WorldCanvas.SetPlaceMode(this, WorldCanvas.PlaceMode.ByLefttop);
                SetBinding(WorldCanvas.LeftProperty, new Binding("Shape.Left"));
                SetBinding(WorldCanvas.TopProperty, new Binding("Shape.Top"));
                return;
            }

            if (ContentControl.Content is CruCircle)
            {
                WorldCanvas.SetPlaceMode(this, WorldCanvas.PlaceMode.ByCenter);
                SetBinding(WorldCanvas.CenterXProperty, new Binding("Shape.Center.X"));
                SetBinding(WorldCanvas.CenterYProperty, new Binding("Shape.Center.Y"));
            }
        }
    }
}
