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
    /// ElectricFieldProperty.xaml 的交互逻辑
    /// </summary>
    public partial class ElectricFieldPropertyDialog : Window
    {
        public ElectricField RelatedElectricField { get; private set; }

        public ElectricFieldPropertyDialog(ElectricField electricField)
        {
            RelatedElectricField = electricField;

            InitializeComponent();

            nameTextBox.Text = electricField.Name;
            shapePropertyControl.ShowProperty(electricField.Shape);
            intensityXTextBox.Text = electricField.Intensity.X.ToString();
            intensityYTextBox.Text = electricField.Intensity.Y.ToString();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            string errorInfo = null;
            var name = nameTextBox.Text;
            var shape = shapePropertyControl.CreateShape(ref errorInfo);
            var intensityX = Common.ParseTextBox(intensityXTextBox, ref errorInfo);
            var intensityY = Common.ParseTextBox(intensityYTextBox, ref errorInfo);

            if (string.IsNullOrEmpty(errorInfo))
            {
                RelatedElectricField.Name = name;
                RelatedElectricField.SetShape(shape);
                RelatedElectricField.Intensity = new Vector(intensityX, intensityY);

                RelatedElectricField.Update();

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
