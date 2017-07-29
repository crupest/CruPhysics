using System.Windows.Media;

namespace CruPhysics.PhysicalObjects
{
    // ReSharper disable once InconsistentNaming
    public static class PhysicalObjectUIResources
    {
        public const int SelectedZIndex = 1000;
        public const int ControllerZIndex = 1001;

        public static Brush NormalStrokeBrush => Brushes.Black;
        public static Brush HoverStrokeBrush => Brushes.Red;
        public static Brush SelectStrokeBrush => Brushes.Blue;

        public const double NormalStrokeThickness = 1.0;
        public const double HoverStrokeThickness = 2.0;
        public const double SelectStrokeThickness = 2.0;
    }
}
