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

namespace CruPhysics
{
    /// <summary>
    /// ShapePropertyControl.xaml 的交互逻辑
    /// </summary>
    public partial class ShapePropertyControl : UserControl
    {
        public ShapePropertyControl()
        {
            InitializeComponent();
        }

        public void ShowProperty(CPShape shape)
        {
            HideAllPane();
            shape.ShowProperty(this);
        }

        public CPShape CreateShape()
        {
            if (rectangleRadioButton.IsChecked.Value)
            {
                return new CPRectangle()
                {
                    Left   = double.Parse(leftTextBox  .Text),
                    Top    = double.Parse(topTextBox   .Text),
                    Right  = double.Parse(rightTextBox .Text),
                    Bottom = double.Parse(bottomTextBox.Text)
                };
            }
            else if (circleRadioButton.IsChecked.Value)
            {
                return new CPCircle()
                {
                    Center = new Point(
                        double.Parse(centerXTextBox.Text),
                        double.Parse(centerYTextBox.Text)
                        ),
                    Radius = double.Parse(radiusTextBox.Text)
                };
            }
            throw new Exception();
        }

        private void HideAllPane()
        {
            rectangleGrid.Visibility = Visibility.Collapsed;
            circleGrid   .Visibility = Visibility.Collapsed;
        }

        private void rectangleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            HideAllPane();
            rectangleGrid.Visibility = Visibility.Visible;
        }

        private void circleRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            HideAllPane();
            circleGrid.Visibility = Visibility.Visible;
        }
    }
}
