using System.Windows;
using System.Windows.Controls;

namespace CruPhysics.Controls
{
    public class WorldCanvas : Panel
    {
        public enum PlaceMode
        {
            ByLefttop,
            ByCenter
        }

        public static readonly DependencyProperty PlaceModeProperty =
            DependencyProperty.RegisterAttached("PlaceMode", typeof(PlaceMode), typeof(WorldCanvas),
            new FrameworkPropertyMetadata(PlaceMode.ByLefttop) {AffectsArrange = true});

        public static readonly DependencyProperty LeftProperty =
            DependencyProperty.RegisterAttached("Left", typeof(double), typeof(WorldCanvas),
                new FrameworkPropertyMetadata(0.0) {AffectsArrange = true});

        public static readonly DependencyProperty TopProperty =
            DependencyProperty.RegisterAttached("Top", typeof(double), typeof(WorldCanvas),
                new FrameworkPropertyMetadata(0.0) {AffectsArrange = true});

        public static readonly DependencyProperty CenterXProperty =
            DependencyProperty.RegisterAttached("CenterX", typeof(double), typeof(WorldCanvas),
                new FrameworkPropertyMetadata(0.0) {AffectsArrange = true});

        public static readonly DependencyProperty CenterYProperty =
            DependencyProperty.RegisterAttached("CenterY", typeof(double), typeof(WorldCanvas),
                new FrameworkPropertyMetadata(0.0) {AffectsArrange = true});

        [AttachedPropertyBrowsableForChildren]
        public static PlaceMode GetPlaceMode(UIElement element)
        {
            return (PlaceMode)element.GetValue(PlaceModeProperty);
        }

        public static void SetPlaceMode(UIElement element, PlaceMode value)
        {
            element.SetValue(PlaceModeProperty, value);
        }

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

        [AttachedPropertyBrowsableForChildren]
        public static double GetCenterX(UIElement element)
        {
            return (double)element.GetValue(CenterXProperty);
        }

        public static void SetCenterX(UIElement element, double value)
        {
            element.SetValue(CenterXProperty, value);
        }

        [AttachedPropertyBrowsableForChildren]
        public static double GetCenterY(UIElement element)
        {
            return (double)element.GetValue(CenterYProperty);
        }

        public static void SetCenterY(UIElement element, double value)
        {
            element.SetValue(CenterYProperty, value);
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
            return new Size();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var center = new Point(finalSize.Width / 2.0, finalSize.Height / 2.0);

            foreach (UIElement child in InternalChildren)
            {
                var lefttop = new Point();

                switch (GetPlaceMode(child))
                {
                    case PlaceMode.ByCenter:
                        lefttop = new Point(
                            center.X + GetCenterX(child) - child.DesiredSize.Width / 2.0,
                            center.Y - GetCenterY(child) - child.DesiredSize.Height / 2.0
                        );
                        break;
                    case PlaceMode.ByLefttop:
                        lefttop = new Point(center.X + GetLeft(child), center.Y - GetTop(child));
                        break;
                }

                child.Arrange(new Rect(lefttop, child.DesiredSize));
            }
            return finalSize;
        }
    }
}
