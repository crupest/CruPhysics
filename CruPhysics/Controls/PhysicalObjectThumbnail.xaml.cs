using System.Diagnostics;
using CruPhysics.PhysicalObjects;
using System.Windows;
using System.Windows.Controls;
using CruPhysics.Windows;

namespace CruPhysics.Controls
{
    /// <summary>
    /// Interaction logic for PhysicalObjectThumbnail.xaml
    /// </summary>
    public partial class PhysicalObjectThumbnail : UserControl
    {
        public static readonly DependencyProperty ObjectProperty =
            DependencyProperty.Register("Object", typeof(PhysicalObject), typeof(PhysicalObjectThumbnail),
                new FrameworkPropertyMetadata(OnObjectChanged));

        private static void OnObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = Window.GetWindow(d) as MainWindow;
            Debug.Assert(window != null);
            ((PhysicalObjectThumbnail) d).Brush.Visual = window.GetPhysicalObjectView((PhysicalObject)e.NewValue);
        }

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
