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
    /// Interaction logic for ObjectPropertyBox.xaml
    /// </summary>
    public partial class ObjectPropertyBox : UserControl
    {
        public static readonly DependencyProperty PropertyNameProperty;
        public static readonly DependencyProperty ValueProperty;
        public static readonly DependencyProperty UnitProperty;

        static ObjectPropertyBox()
        {
            PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(0.0));
            UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(ObjectPropertyBox), new FrameworkPropertyMetadata(""));
        }

        public ObjectPropertyBox()
        {
            InitializeComponent();
        }

        public string PropertyName
        {
            get
            {
                 return (string)GetValue(PropertyNameProperty);
            }
            set
            {
                SetValue(PropertyNameProperty, value);
            }
        }

        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        public event KeyEventHandler ValueTextBoxKeyDown
        {
            add
            {
                ValueTextBox.KeyDown += value;
            }
            remove
            {
                ValueTextBox.KeyDown -= value;
            }
        }
    }
}
