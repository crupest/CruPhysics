using CruPhysics.PhysicalObjects;
using System.Windows;
using System.Windows.Controls;

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
