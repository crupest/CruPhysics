using CruPhysics.Shapes;
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

namespace CruPhysics.Controls
{
    /// <summary>
    /// Interaction logic for CirclePropertyControl.xaml
    /// </summary>
    public partial class CirclePropertyControl : UserControl
    {
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register("Shape", typeof(CruCircle), typeof(CirclePropertyControl), new FrameworkPropertyMetadata());


        public CirclePropertyControl()
        {
            InitializeComponent();
        }

        public CruCircle Shape
        {
            get
            {
                return (CruCircle)GetValue(ShapeProperty);
            }
            set
            {
                SetValue(ShapeProperty, value);
            }
        }
    }
}
