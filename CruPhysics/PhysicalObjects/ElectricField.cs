using CruPhysics.Windows;
using System;
using System.Windows;

namespace CruPhysics.PhysicalObjects
{
    public class ElectricField : Field
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 50, RunRank = 50 };


        private BindableVector intensity = new BindableVector();

        public ElectricField()
        {
            Name = "电场";
        }

        public BindableVector Intensity
        {
            get
            {
                return intensity;
            }
        }

        public override void CalculateEffect(MovingObject movingObject, TimeSpan no_use)
        {
            movingObject.Forces.Add(new Force(
                    Intensity.X * movingObject.Charge,
                    Intensity.Y * movingObject.Charge
                ));
        }

        public override Window CreatePropertyWindow()
        {
            return new ElectricFieldPropertyDialog(this);
        }
    }
}
