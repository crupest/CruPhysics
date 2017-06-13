using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using CruPhysics.Windows;
using System.Windows.Controls;
using System.Globalization;

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

    public class NumberValidationRule : ValidationRule
    {
        protected double number;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                number = double.Parse(value.ToString());
            }
            catch (FormatException)
            {
                return new ValidationResult(false, "不是一个数字！");
            }
            catch (OverflowException)
            {
                return new ValidationResult(false, "超出范围！");
            }
            return new ValidationResult(true, null);
        }
    }
}
