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
    /// Interaction logic for PhysicalObjectCommonPropertyControl.xaml
    /// </summary>
    public partial class PhysicalObjectCommonPropertyControl : UserControl
    {
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(PhysicalObject), typeof(PhysicalObjectCommonPropertyControl), new FrameworkPropertyMetadata(null));

        public PhysicalObjectCommonPropertyControl()
        {
            InitializeComponent();
        }

        public PhysicalObject Object
        {
            get
            {
                return (PhysicalObject)GetValue(ObjectProperty);
            }
            set
            {
                SetValue(ObjectProperty, value);
            }
        }
    }
}
