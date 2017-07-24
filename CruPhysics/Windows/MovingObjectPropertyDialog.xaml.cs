using CruPhysics.PhysicalObjects;
using System.Windows;

namespace CruPhysics.Windows
{
    /// <summary>
    /// MovingObjectProperty.xaml 的交互逻辑
    /// </summary>
    public partial class MovingObjectPropertyDialog : Window
    {
        public MovingObject RelatedMovingObject { get; private set; }

        public MovingObjectPropertyDialog(MovingObject movingObject)
        {
            RelatedMovingObject = movingObject;

            InitializeComponent();
        }
    }
}
