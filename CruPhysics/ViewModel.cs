using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using CruPhysics.Windows;
using System.Windows.Controls;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace CruPhysics
{
    public class MainViewModel : NotifyPropertyChangedObject
    {
        private MainWindow window;
        private Scene scene;

        public MainViewModel(MainWindow window)
        {
            this.window = window;
            scene = new Scene(window);
        }

        public MainWindow Window
        {
            get
            {
                return window;
            }
        }

        public Scene Scene
        {
            get
            {
                return scene;
            }
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }


    public class NumberValidationRule : ValidationRule
    {
        protected virtual ValidationResult Validate(double value)
        {
            return new ValidationResult(true, null);
        }

        public sealed override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double result;
            try
            {
                result = double.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "不是一个数字!");
            }
            catch (OverflowException)
            {
                return new ValidationResult(false, "超出范围！");
            }
            return Validate(result);
        }
    }

    public class PositiveValidationRule : NumberValidationRule
    {
        public PositiveValidationRule()
        {
            Info = "必须是一个正数！";
        }

        public string Info { get; set; }

        protected override ValidationResult Validate(double value)
        {
            if (value <= 0.0)
                return new ValidationResult(false, Info);
            return base.Validate(value);
        }
    }

    public class PositiveOrZeroValidationRule : NumberValidationRule
    {
        public PositiveOrZeroValidationRule()
        {
            Info = "必须是一个非负数！";
        }

        public string Info { get; set; }

        protected override ValidationResult Validate(double value)
        {
            if (value < 0.0)
                return new ValidationResult(false, Info);
            return base.Validate(value);
        }
    }
}
