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

namespace CruPhysics
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

            nameTextBox.Text = magneticField.Name;
            shapePropertyControl.ShowProperty(magneticField.Shape);
            fluxDensityTextBox.Text = magneticField.FluxDensity.ToString();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            string errorInfo = null;
            var name = nameTextBox.Text;
            var shape = shapePropertyControl.CreateShape(ref errorInfo);
            var fluxDensity = Common.ParseTextBox(fluxDensityTextBox, ref errorInfo);

            if (string.IsNullOrEmpty(errorInfo))
            {
                RelatedMagneticField.Name = name;
                RelatedMagneticField.SetShape(shape);
                RelatedMagneticField.FluxDensity = fluxDensity;

                Close();
            }
            else
            {
                MessageBox.Show(errorInfo, "错误！", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).Background = Brushes.White;
        }
    }
}
