using System;
using System.Windows;
using System.Windows.Media;

namespace CruPhysics
{
    public class BindableVector : NotifyPropertyChangedObject
    {
        private double x;
        private double y;

        public BindableVector()
        {

        }

        public BindableVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public BindableVector(Vector vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        public double X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChangedEvent("X");
            }
        }

        public double Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        public void Set(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public void Add(Vector vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static implicit operator Vector(BindableVector vector)
        {
            return new Vector(vector.x, vector.y);
        }
    }

    public class BindablePoint : NotifyPropertyChangedObject
    {
        private double x;
        private double y;

        public BindablePoint()
        {

        }

        public BindablePoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public BindablePoint(Point point)
        {
            x = point.X;
            y = point.Y;
        }

        public double X
        {
            get => x;
            set
            {
                x = value;
                RaisePropertyChangedEvent("X");
            }
        }

        public double Y
        {
            get => y;
            set
            {
                y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        public void Set(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public void Move(Vector vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static implicit operator Point(BindablePoint point)
        {
            return new Point(point.x, point.y);
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
            DependencyObject GetParent(DependencyObject e)
            {
                if (e is Visual)
                {
                    var p = VisualTreeHelper.GetParent(e);
                    if (p != null)
                        return p;
                }
                return LogicalTreeHelper.GetParent(e);
            }

            var parent = includeSelf ? element : GetParent(element);
            while (true)
            {
                if (parent == null || predicate(parent))
                {
                    return parent;
                }
                parent = GetParent(parent);
            }
        }

        public static TResult FindAncestor<TResult>(DependencyObject element, Func<DependencyObject, bool> predicate,
            bool includeSelf = false) where TResult : DependencyObject
        {
            return FindAncestor(element, predicate, includeSelf) as TResult;
        }

        public static double GetDistance(Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        private static readonly Random random = new Random();

        public static Color GetRamdomColor()
        {
            return Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
        }
    }
}
