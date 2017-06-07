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

using CruPhysics.Shapes;
using System.ComponentModel;

namespace CruPhysics.Controls
{
    /// <summary>
    /// ShapePropertyControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShapePropertyControl : UserControl
    {
        public static readonly DependencyProperty ShapeProperty;

        static ShapePropertyControl()
        {
            ShapeProperty = DependencyProperty.Register("Shape", typeof(CruShape), typeof(ShapePropertyControl), new FrameworkPropertyMetadata() { PropertyChangedCallback = ShapePropertyChanged});
        }

        private static void ShapePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (ShapePropertyControl) sender;

            if (args.NewValue is CruRectangle)
            {
                control.RectangleRadioButton.IsChecked = true;
                control.PropertyControl.Content = new RectanglePropertyControl((CruRectangle)args.NewValue);
            }
            else if (args.NewValue is CruCircle)
            {
                control.CircleRadioButton.IsChecked = true;
                control.PropertyControl.Content = new CirclePropertyControl((CruCircle)args.NewValue);
            }
        }


        public ShapePropertyControl()
        {
            InitializeComponent();
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
                Shape = new CruCircle();
        }
    }
}
