using CruPhysics.Controls;
using System;
using System.Windows;

namespace CruPhysics.Shapes
{
    public sealed class CruCircle : CruShape
    {
        private BindablePoint center = new BindablePoint();
        private double radius = 10.0;

        public CruCircle()
        {

        }

        public override SelectionBox CreateSelectionBox()
        {
            return new CircleSelectionBox(this);
        }

        public BindablePoint Center => center;

        public double Radius
        {
            get => radius;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException
                        ("Radius", value, "Radius can't be smaller than 0.");

                radius = value;
                RaisePropertyChangedEvent(nameof(Radius));
            }
        }

        public override void Move(Vector vector)
        {
            Center.Move(vector);
        }

        public override bool IsPointInside(Point point)
        {
            return Math.Pow(point.X - Center.X, 2) +
                Math.Pow(point.Y - Center.Y, 2) <= Math.Pow(Radius, 2);
        }
    }
}
