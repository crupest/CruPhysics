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
}
