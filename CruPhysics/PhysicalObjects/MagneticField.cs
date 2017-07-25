using CruPhysics.Windows;
using System;
using System.Windows;

namespace CruPhysics.PhysicalObjects
{
    public class MagneticField : Field
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 50, RunRank = 49 };


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
