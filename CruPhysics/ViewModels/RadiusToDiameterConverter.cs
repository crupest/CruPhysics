using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace CruPhysics.ViewModels
{
    [ValueConversion(typeof(double), typeof(double))]
    public class RadiusToDiameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((double) value) * 2.0;
            return 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                return ((double) value) / 2.0;
            return 0.0;
        }
    }
}
