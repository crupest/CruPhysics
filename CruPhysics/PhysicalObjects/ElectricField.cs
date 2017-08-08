using CruPhysics.Windows;
using System;
using System.Windows;
using CruPhysics.PhysicalObjects.Views;

namespace CruPhysics.PhysicalObjects
{
    [PhysicalObjectMetadata(ZIndex = 50, RunRank = 50, ViewType = typeof(FieldView))]
    public class ElectricField : Field
    {
        public ElectricField()
        {
            Name = "电场";
        }

        public BindableVector Intensity { get; } = new BindableVector();

        public override void CalculateEffect(MovingObject movingObject, TimeSpan noUse)
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
