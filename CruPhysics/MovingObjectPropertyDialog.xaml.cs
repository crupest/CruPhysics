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
            RelatedMovingObject.Name = nameTextBox.Text;
            RelatedMovingObject.Position = new Point(
                double.Parse(positionXTextBox.Text),
                double.Parse(positionYTextBox.Text));
            RelatedMovingObject.Radius = double.Parse(radiusTextBox.Text);
            RelatedMovingObject.Velocity = new Vector(
                double.Parse(velocityXTextBox.Text),
                double.Parse(velocityYTextBox.Text));
            RelatedMovingObject.Mass = double.Parse(massTextBox.Text);
            RelatedMovingObject.Charge = double.Parse(chargeTextBox.Text);

            RelatedMovingObject.Update();

            Close();
        }

        private void cancleButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
