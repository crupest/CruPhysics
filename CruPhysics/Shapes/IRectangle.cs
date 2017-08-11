using System.Windows;

namespace CruPhysics.Shapes
{
    public interface IRectangle : IShape
    {
        double Left { get; set; }
        double Top { get; set; }
        double Width { get; set; }
        double Height { get; set; }
    }

    public static class RectangleExtension
    {
        public static double GetRight(this IRectangle rectangle)
        {
            return rectangle.Left + rectangle.Width;
        }

        public static double GetBottom(this IRectangle rectangle)
        {
            return rectangle.Top - rectangle.Height;
        }

        public static Point GetLefttop(this IRectangle rectangle)
        {
            return new Point(rectangle.Left, rectangle.Top);
        }

        public static Point GetRighttop(this IRectangle rectangle)
        {
            return new Point(rectangle.GetRight(), rectangle.Top);
        }

        public static Point GetLeftbottom(this IRectangle rectangle)
        {
            return new Point(rectangle.Left, rectangle.GetBottom());
        }

        public static Point GetRightbottom(this IRectangle rectangle)
        {
            return new Point(rectangle.GetRight(), rectangle.GetBottom());
        }

        public static Point GetCenter(this IRectangle rectangle)
        {
            return new Point(rectangle.Left + rectangle.Width / 2.0, rectangle.Top - rectangle.Height / 2.0);
        }

        public static bool IsPointInside(this IRectangle rectangle, Point point)
        {
            return
                point.X > rectangle.Left &&
                point.X < rectangle.GetRight() &&
                point.Y > rectangle.GetBottom() &&
                point.Y < rectangle.Top;
        }
    }
}
