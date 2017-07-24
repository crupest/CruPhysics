using CruPhysics.Shapes;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CruPhysics.Controls
{
    public partial class ShapePropertyControl : UserControl
    {
        public static readonly DependencyProperty ShapeProperty;

        static ShapePropertyControl()
        {
            ShapeProperty = DependencyProperty.Register("Shape", typeof(CruShape), typeof(ShapePropertyControl), new FrameworkPropertyMetadata());
        }


        public ShapePropertyControl()
        {
            InitializeComponent();

            var descriptor = DependencyPropertyDescriptor.FromProperty(ShapeProperty, GetType());
            descriptor.AddValueChanged(this, (sender, args) =>
            {
                var shape = Shape;
                if (shape is CruRectangle)
                    RectangleRadioButton.IsChecked = true;
                else if (shape is CruCircle)
                    CircleRadioButton.IsChecked = true;
            });
        }

        public CruShape Shape
        {
            get
            {
                return (CruShape)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }

        private void RectangleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!(Shape is CruRectangle))
                Shape = new CruRectangle();
        }

        private void CircleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!(Shape is CruCircle))
                Shape = new CruCircle() { Radius = 50 };
        }
    }
}
