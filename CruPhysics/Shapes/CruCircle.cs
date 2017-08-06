using CruPhysics.Controls;
using System;
using System.Windows;

namespace CruPhysics.Shapes
{
    public sealed class CruCircle : CruShape, ICircle
    {
        private double radius = 10.0;

        public BindablePoint Center { get; } = new BindablePoint();

        public double Radius
        {
            get => radius;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException
                        (nameof(value), value, "Radius can't be smaller than 0.");

                radius = value;
                RaisePropertyChangedEvent(nameof(Radius));
                RaisePropertyChangedEvent(nameof(Diameter));
            }
        }

        public double Diameter => Radius * 2.0;

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
