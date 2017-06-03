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

using CruPhysics.Shapes;

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

        public void ShowProperty(CruShape shape)
        {
            HideAllPane();
            shape.ShowProperty(this);
        }

        public CruShape CreateShape(ref string errorInfo)
        {
            if (rectangleRadioButton.IsChecked.Value)
            {
                string internalErrorInfo1 = null;
                var left = Common.ParseTextBox(leftTextBox, ref internalErrorInfo1);
                var right = Common.ParseTextBox(rightTextBox, ref internalErrorInfo1);
                if (string.IsNullOrEmpty(internalErrorInfo1) && left >= right)
                    internalErrorInfo1 += "左边界必须小于右边界！";
                string internalErrorInfo2 = null;
                var top = Common.ParseTextBox(topTextBox, ref internalErrorInfo2);
                var bottom = Common.ParseTextBox(bottomTextBox, ref internalErrorInfo2);
                if (string.IsNullOrEmpty(internalErrorInfo2) && top <= bottom)
                    internalErrorInfo2 += "上边界必须大于下边界！";
                var internalErrorInfo = internalErrorInfo1 + internalErrorInfo2;
                errorInfo += internalErrorInfo;
                if (string.IsNullOrEmpty(internalErrorInfo))
                    return new Rectangle()
                    {
                        Left = left,
                        Top = top,
                        Right = right,
                        Bottom = bottom
                    };
            }
            else if (circleRadioButton.IsChecked.Value)
            {
                string internalError = null;
                var centerX = Common.ParseTextBox(centerXTextBox, ref internalError);
                var centerY = Common.ParseTextBox(centerYTextBox, ref internalError);
                var radius = Common.ParseTextBox(radiusTextBox, x => x > 0.0, ref internalError);
                errorInfo += internalError;
                if (string.IsNullOrEmpty(internalError))
                {
                    var circle = new Circle() { Radius = radius };
                    circle.Center.X = centerX;
                    circle.Center.Y = centerY;
                    return circle;
                }
            }
            return null;
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

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((TextBox)sender).Background = Brushes.White;
        }
    }
}
