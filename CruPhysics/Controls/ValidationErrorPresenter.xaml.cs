using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CruPhysics.Controls
{
    /// <summary>
    /// Interaction logic for ValidationErrorPresenter.xaml
    /// </summary>
    public partial class ValidationErrorPresenter : UserControl
    {
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(DependencyObject), typeof(ValidationErrorPresenter));

        public ValidationErrorPresenter()
        {
            InitializeComponent();

            var descriptor = DependencyPropertyDescriptor.FromProperty(TargetProperty, typeof(ValidationErrorPresenter));
            descriptor.AddValueChanged(this, (sender, args) =>
            {
                Binding binding1 = new Binding("(Validation.HasError)") { Source = Target, Converter = new BoolToVisibilityConverter() };
                SetBinding(VisibilityProperty, binding1);

                Binding binding2 = new Binding("(Validation.Errors).CurrentItem.ErrorContent") { Source = Target };
                TheTextBlock.SetBinding(TextBlock.TextProperty, binding2);
            });
        }

        public DependencyObject Target
        {
            get
            {
                return (DependencyObject)GetValue(TargetProperty);
            }
            set
            {
                SetValue(TargetProperty, value);
            }
        }
    }
}
