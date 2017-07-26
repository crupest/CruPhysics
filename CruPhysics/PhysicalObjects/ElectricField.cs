﻿using CruPhysics.Windows;
using System;
using System.Windows;

namespace CruPhysics.PhysicalObjects
{
    public class ElectricField : Field
    {
        // ReSharper disable once InconsistentNaming
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 50, RunRank = 50 };


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