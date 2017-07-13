using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CruPhysics
{
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        public NotifyPropertyChangedObject()
        {
            IsNotifying = true;
        }

        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        public bool IsNotifying { get; set; }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (IsNotifying)
                propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class PropertyManager
    {
        public static object Object { get; set; }

        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> property)
        {
            var memberExpression = (MemberExpression)property.Body;
            return memberExpression.Member.Name;
        }

        public static object GetPropertyValue(object target, string propertyName)
        {
            return target.GetType().GetProperty(propertyName).GetValue(target);
        }

        public static void SetPropertyValue(object target, string propertyName, object value)
        {
            target.GetType().GetProperty(propertyName).SetValue(target, value);
        }

        public static void OneWayBind(INotifyPropertyChanged source, string sourcePropertyName, object target, string targetPropertyName)
        {
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == sourcePropertyName)
                {
                    SetPropertyValue(target, targetPropertyName, GetPropertyValue(source, sourcePropertyName));
                }
            };
        }

        public static void TwoWayBind(INotifyPropertyChanged source, string sourcePropertyName, INotifyPropertyChanged target, string targetPropertyName)
        {
            bool b = true;
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == sourcePropertyName && b)
                {
                    b = !b;
                    SetPropertyValue(target, targetPropertyName, GetPropertyValue(source, sourcePropertyName));
                }
                else
                {
                    b = !b;
                }
            };
            target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == targetPropertyName && b)
                {
                    b = !b;
                    SetPropertyValue(source, sourcePropertyName, GetPropertyValue(target, targetPropertyName));
                }
                else
                {
                    b = !b;
                }
            };
        }
    }

    public static class Common
    {
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
        /// 计算向量与x轴之间的角度，顺时针
        /// </summary>
        public static double GetAngleBetweenXAxis(Vector vector)
        {
            if (vector.X == 0.0)
            {
                if (vector.Y == 0.0)
                    return 0.0;
                else if (vector.Y > 0.0)
                    return Math.PI * 3.0 / 2.0;
                else
                    return Math.PI / 2.0;
            }

            var a = Math.Atan(vector.Y / vector.X);
            if (vector.X < 0.0)
                a += Math.PI;
            return Math.PI * 2.0 - a;
        }

        public static double GetAngle(Vector vector1, Vector vector2)
        {
            return GetAngleBetweenXAxis(vector2) - GetAngleBetweenXAxis(vector1);
        }

        public static Point TransformPoint(Point point)
        {
            return new Point(point.X, -point.Y);
        }

        public static DependencyObject FindAncestor(DependencyObject element, Func<DependencyObject, bool> predicate, bool includeSelf = false)
        {
            Func<DependencyObject, DependencyObject> getParent = (e) =>
            {
                if (e is Visual)
                    return VisualTreeHelper.GetParent(e);
                else
                    return LogicalTreeHelper.GetParent(e);
            };

            var parent = includeSelf ? element : getParent(element);
            while (true)
            {
                if (parent == null || predicate(parent))
                    return parent;
                parent = getParent(parent);
            }
        }

        public static double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        private static Random random = new Random();

        public static Color GetRamdomColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }
    }
}
