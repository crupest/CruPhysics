using CruPhysics.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
using CruPhysics.PhysicalObjects.Views;

namespace CruPhysics.PhysicalObjects
{
    [PhysicalObjectMetadata(ZIndex = 100, RunRank = 100, ViewType = typeof(MovingObjectView))]
    public class MovingObject : PhysicalObject
    {
        private double radius = 10.0;
        private double mass = 1.0;
        private double charge;

        private readonly List<Force> forces = new List<Force>();
        private readonly MotionTrail trail = new MotionTrail();

        public MovingObject()
        {
            Name = "运动对象";
        }

        public BindablePoint Position { get; } = new BindablePoint();

        public double Radius
        {
            get => radius;
            set
            {
                radius = value;
                RaisePropertyChangedEvent(nameof(Radius));
                RaisePropertyChangedEvent(nameof(Diameter));
            }
        }

        public double Diameter => Radius * 2.0;

        public BindableVector Velocity { get; } = new BindableVector();

        public double Mass
        {
            get => mass;
            set
            {
                mass = value;
                RaisePropertyChangedEvent(nameof(Mass));
            }
        }

        public double Charge
        {
            get => charge;
            set
            {
                charge = value;
                RaisePropertyChangedEvent(nameof(Charge));
            }
        }

        public override void Move(Vector vector)
        {
            Position.Move(vector);
        }


        public IList<Force> Forces => forces;

        public override Window CreatePropertyWindow()
        {
            return new MovingObjectPropertyDialog(this);
        }

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
            scene.BeginRunningEvent += OnBeginRunning;
            scene.RefreshEvent += OnRefresh;
            scene.AfterOneScanEvent += AfterOneScan;
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
            scene.BeginRunningEvent -= OnBeginRunning;
            scene.RefreshEvent -= OnRefresh;
            scene.AfterOneScanEvent -= AfterOneScan;
        }

        private void OnBeginRunning(object sender, EventArgs args)
        {
            StoreProperty();
            trail.AddPoint(Position);
        }

        private void OnRefresh(object sender, EventArgs args)
        {
            RecoverProperty();
            trail.Clear();
        }

        private void AfterOneScan(object sender, SceneScanEventArgs args)
        {
            trail.AddPoint(Position);
        }


        private Point originalPosition;
        private Vector originalVelocity;

        private void StoreProperty()
        {
            originalPosition = Position;
            originalVelocity = Velocity;
        }

        private void RecoverProperty()
        {
            Position.Set(originalPosition);
            Velocity.Set(originalVelocity);
        }

        public override void Run(Scene scene, TimeSpan time)
        {
            var t = time.TotalSeconds;
            var force = new Vector();
            foreach (var i in forces)
            {
                force.X += i.X;
                force.Y += i.Y;
            }
            var acceleration = force / Mass;
            Move((Vector)Velocity * t + acceleration * Math.Pow(t, 2) / 2.0);
            Velocity.Add(acceleration * t);
        }
    }
}
