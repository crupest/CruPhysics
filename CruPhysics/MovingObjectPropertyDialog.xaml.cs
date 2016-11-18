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
    /// MovingObjectProperty.xaml 的交互逻辑
    /// </summary>
    public partial class MovingObjectPropertyDialog : Window
    {
        public MovingObject RelatedMovingObject { get; private set; }

        public MovingObjectPropertyDialog(MovingObject movingObject)
        {
            this.RelatedMovingObject = movingObject;

            InitializeComponent();

            nameTextBox     .Text = movingObject.Name;
            positionXTextBox.Text = movingObject.Position.X.ToString();
            positionYTextBox.Text = movingObject.Position.Y.ToString();
            radiusTextBox   .Text = movingObject.Radius    .ToString();
            velocityXTextBox.Text = movingObject.Velocity.X.ToString();
            velocityYTextBox.Text = movingObject.Velocity.Y.ToString();
            massTextBox     .Text = movingObject.Mass      .ToString();
            chargeTextBox   .Text = movingObject.Charge    .ToString();
        }

        private void confirmButton_Click(object sender, RoutedEventArgs e)
        {
            string info = null;

            var name = nameTextBox.Text;
            var positionX = Common.ParseTextBox(positionXTextBox, ref info);
            var positionY = Common.ParseTextBox(positionYTextBox, ref info);
            var radius    = Common.ParseTextBox(radiusTextBox, x => x > 0.0, ref info);
            var velocityX = Common.ParseTextBox(velocityXTextBox, ref info);
            var velocityY = Common.ParseTextBox(velocityYTextBox, ref info);
            var mass      = Common.ParseTextBox(massTextBox, x => x > 0.0, ref info);
            var charge    = Common.ParseTextBox(chargeTextBox, ref info);

            if (string.IsNullOrEmpty(info))
            {
                RelatedMovingObject.Position = new Point(positionX, positionY);
                RelatedMovingObject.Radius = radius;
                RelatedMovingObject.Velocity = new Vector(velocityX, velocityY);
                RelatedMovingObject.Mass = mass;
                RelatedMovingObject.Charge = charge;

                RelatedMovingObject.Update();

                Close();
            }
            else
            {
                MessageBox.Show(info, "错误！", MessageBoxButton.OK, MessageBoxImage.Error);
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
