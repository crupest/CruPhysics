using CruPhysics.Windows;
using System;
using System.Windows;
using CruPhysics.PhysicalObjects.Views;

namespace CruPhysics.PhysicalObjects
{
    [PhysicalObjectMetadata(ZIndex = 50, RunRank = 49, ViewType = typeof(FieldView))]
    public class MagneticField : Field
    {
        private double fluxDensity;

        public MagneticField()
        {
            Name = "磁场";
        }

        /// <summary>
        /// 为正时垂直纸面向里，为负时垂直于纸面向外
        /// </summary>
        public double FluxDensity
        {
            get => fluxDensity;
            set
            {
                fluxDensity = value;
                RaisePropertyChangedEvent(nameof(FluxDensity));
            }
        }

        public override void CalculateEffect(MovingObject movingObject, TimeSpan time)
        {
            //qvB = mvω => ω = qB/m => α = ωt = qBt/m
            movingObject.Velocity.Set(Common.Rotate(movingObject.Velocity,
                -time.TotalSeconds * movingObject.Charge * FluxDensity / movingObject.Mass));
        }

        public override Window CreatePropertyWindow()
        {
            return new MagneticFieldPropertyDialog(this);
        }
    }
}
