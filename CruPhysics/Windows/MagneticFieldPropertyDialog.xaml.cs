using CruPhysics.PhysicalObjects;
using System.Windows;

namespace CruPhysics.Windows
{
    /// <summary>
    /// MagneticFieldPropertyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class MagneticFieldPropertyDialog : Window
    {
        public MagneticField RelatedMagneticField { get; private set; }

        public MagneticFieldPropertyDialog(MagneticField magneticField)
        {
            RelatedMagneticField = magneticField;

            InitializeComponent();
        }
    }
}
