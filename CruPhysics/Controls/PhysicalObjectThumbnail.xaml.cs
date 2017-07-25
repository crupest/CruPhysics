using CruPhysics.PhysicalObjects;
using System.Windows;
using System.Windows.Controls;

namespace CruPhysics.Controls
{
    /// <summary>
    /// Interaction logic for PhysicalObjectThumbnail.xaml
    /// </summary>
    public partial class PhysicalObjectThumbnail : UserControl
    {
        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(PhysicalObject), typeof(PhysicalObjectThumbnail), new FrameworkPropertyMetadata(null));

        public PhysicalObjectThumbnail()
        {
            InitializeComponent();
        }

        public PhysicalObject Object
        {
            get => (PhysicalObject)GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }
    }
}
