using System.Windows;
using System.Windows.Controls;

namespace CruPhysics.Controls
{
    public class WorldCanvas : Panel
    {
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached("Left", typeof(double), typeof(WorldCanvas), new FrameworkPropertyMetadata(0.0) { AffectsArrange = true });
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached("Top", typeof(double), typeof(WorldCanvas), new FrameworkPropertyMetadata(0.0) { AffectsArrange = true });

        [AttachedPropertyBrowsableForChildren]
        public static double GetLeft(UIElement element)
        {
            return (double) element.GetValue(LeftProperty);
        }

        public static void SetLeft(UIElement element, double value)
        {
            element.SetValue(LeftProperty, value);
        }

        [AttachedPropertyBrowsableForChildren]
        public static double GetTop(UIElement element)
        {
            return (double) element.GetValue(TopProperty);
        }

        public static void SetTop(UIElement element, double value)
        {
            element.SetValue(TopProperty, value);
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var center = new Point(finalSize.Width / 2.0, finalSize.Height / 2.0);
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(
                    new Point(center.X + GetLeft(child), center.Y - GetTop(child)),
                    child.DesiredSize)
                );
            }
            return finalSize;
        }
    }
}
