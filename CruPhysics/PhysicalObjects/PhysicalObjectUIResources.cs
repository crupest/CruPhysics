using System.Collections.Generic;
using System.Windows.Media;

namespace CruPhysics.PhysicalObjects
{
    public static class PhysicalObjectUIResources
    {
        public const int SelectedZIndex = 1000;
        public const int ControllerZIndex = 1001;

        static PhysicalObjectUIResources()
        {
            strokeBrushes.Add(SelectionState.Normal, Brushes.Black);
            strokeBrushes.Add(SelectionState.Hover, Brushes.Red);
            strokeBrushes.Add(SelectionState.Select, Brushes.Blue);

            strokeThickness.Add(SelectionState.Normal, 1.0);
            strokeThickness.Add(SelectionState.Hover, 2.0);
            strokeThickness.Add(SelectionState.Select, 2.0);
        }

        private static readonly Dictionary<SelectionState, Brush> strokeBrushes = new Dictionary<SelectionState, Brush>();
        private static readonly Dictionary<SelectionState, double> strokeThickness = new Dictionary<SelectionState, double>();

        public static IDictionary<SelectionState, Brush> StrokeBrushes => strokeBrushes;
        public static IDictionary<SelectionState, double> StrokeThickness => strokeThickness;
    }
}
