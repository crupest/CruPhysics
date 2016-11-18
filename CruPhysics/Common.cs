using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CruPhysics
{
    public static class Common
    {


        public delegate bool CheckValue(double value);

        public static double ParseTextBox(TextBox textBox, ref string info)
        {
            return ParseTextBox(textBox, null, ref info);
        }

        public static double ParseTextBox(TextBox textBox, CheckValue checkValue, ref string info)
        {
            if (textBox == null)
                throw new ArgumentNullException("textBox");
            if (!(textBox.Tag is string))
                throw new Exception("The Tag of textbox is not a string.");

            double result = 0.0;
            try
            {
                result = double.Parse(textBox.Text);
                if (checkValue != null && !checkValue(result))
                    throw new OverflowException();
            }
            catch (FormatException)
            {
                textBox.Background = Brushes.Red;
                info += (string)textBox.Tag + "不是一个数字！\n";
            }
            catch (OverflowException)
            {
                textBox.Background = Brushes.Red;
                info += (string)textBox.Tag + "超出范围！\n";
            }
            return result;
        }
    }
}
