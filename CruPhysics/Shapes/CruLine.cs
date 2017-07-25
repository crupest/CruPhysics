using CruPhysics.Controls;
using System;
using System.Windows;

namespace CruPhysics.Shapes
{
    public sealed class CruLine : CruShape
    {
        private BindablePoint point1 = new BindablePoint();
        private BindablePoint point2 = new BindablePoint();

        public CruLine()
        {

        }

        public BindablePoint Point1 => point1;

        public BindablePoint Point2 => point2;

        public override SelectionBox CreateSelectionBox()
        {
            throw new NotImplementedException();
        }

        public override bool IsPointInside(Point point)
        {
            System.Diagnostics.Debug.WriteLine("Try to test if a point is in a line.");
            return false;
        }

        public override void Move(Vector vector)
        {
            point1.Move(vector);
            point2.Move(vector);
        }
    }
}
