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
using System.Windows.Shapes;

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
