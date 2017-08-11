using System;
using System.Windows;

namespace CruPhysics.Shapes
{
    public interface ICircle : IShape
    {
        BindablePoint Center { get; }
        double Radius { get; set; }
    }

    public static class CircleExtension
    {
        public static bool IsPointInside(this ICircle circle, Point point)
        {
            return Math.Pow(point.X - circle.Center.X, 2) +
                   Math.Pow(point.Y - circle.Center.Y, 2) <= Math.Pow(circle.Radius, 2);
        }
    }
}
