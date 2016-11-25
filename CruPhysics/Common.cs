﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        /// <summary>
        /// 顺时针旋转一个向量一定角度
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="angle">顺时针旋转的角度</param>
        /// <returns>旋转得到的向量</returns>
        public static Vector Rotate(Vector vector, double angle)
        {
            double newAngle;
            if (vector.X == 0.0)
            {
                if (vector.Y > 0.0)
                    newAngle = Math.PI / 2.0 - angle;
                else
                    newAngle = Math.PI * 3.0 / 2.0 - angle;
            }
            else
            {
                newAngle = Math.Atan(vector.Y / vector.X) - angle;
                if (vector.X < 0.0)      //It took me over one hour to
                    newAngle += Math.PI; //think about these two line.
            }
            double length = vector.Length;
            return new Vector(length * Math.Cos(newAngle), length * Math.Sin(newAngle));
        }

        /// <summary>
        /// 顺时针旋转一个向量一定角度
        /// </summary>
        /// <param name="vector">要旋转的向量</param>
        /// <param name="angle">顺时针旋转的角度</param>
        public static void Rotate(ref Vector vector, double angle)
        {
            vector = Rotate(vector, angle);
        }
    }
}
