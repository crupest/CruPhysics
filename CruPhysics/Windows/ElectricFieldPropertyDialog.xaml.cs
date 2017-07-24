using CruPhysics.PhysicalObjects;
using System.Windows;

namespace CruPhysics.Windows
{
    /// <summary>
    /// ElectricFieldProperty.xaml 的交互逻辑
    /// </summary>
    public partial class ElectricFieldPropertyDialog : Window
    {
        public ElectricField RelatedElectricField { get; private set; }

        public ElectricFieldPropertyDialog(ElectricField electricField)
        {
            RelatedElectricField = electricField;

            InitializeComponent();
        }
    }
}
