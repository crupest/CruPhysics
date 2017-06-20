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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CruPhysics.Controls
{
    [ContentProperty("Content")]
    public class ValidationContainer : Control
    {
        public static readonly DependencyProperty ContentProperty;

        static ValidationContainer()
        {
            ContentProperty = DependencyProperty.Register("Content", typeof(DependencyObject), typeof(ValidationContainer), new FrameworkPropertyMetadata(null));

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ValidationContainer), new FrameworkPropertyMetadata(typeof(ValidationContainer)));
        }

        public ValidationContainer()
        {
            var descriptor = DependencyPropertyDescriptor.FromProperty(ContentProperty, GetType());
            descriptor.AddValueChanged(this, (sender, args) => ErrorTextBoxBind());
        }

        public DependencyObject Content
        {
            get
            {
                return (DependencyObject)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        private void ErrorTextBoxBind()
        {
            if (GetTemplateChild("ErrorTextBlock") is FrameworkElement errorTextBox)
            {
                var binding1 = new Binding("(Validation.HasError)")
                {
                    Source = Content,
                    Converter = new BoolToVisibilityConverter(),
                };
                errorTextBox.SetBinding(TextBlock.VisibilityProperty, binding1);

                var binding2 = new Binding("(Validation.Errors)[0].ErrorContent")
                {
                    Source = Content,
                };
                errorTextBox.SetBinding(TextBlock.TextProperty, binding2);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ErrorTextBoxBind();
        }
    }
}
