using CruPhysics.Windows;
using System;
using System.Collections.Generic;
using System.Windows;
namespace CruPhysics.PhysicalObjects
{
    public class MovingObject : PhysicalObject
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 100, RunRank = 100 };

        private BindablePoint position = new BindablePoint();
        private double radius;
        private BindableVector velocity = new BindableVector();
        private double mass;
        private double charge;

        private List<Force> forces = new List<Force>();
        private MotionTrail trail = new MotionTrail();

        public MovingObject()
        {
            Name = "运动对象";
            Radius = 10.0;
            Mass = 1.0;
        }

        public BindablePoint Position => position;

        public double Radius
        {
            get => radius;
            set
            {
                radius = value;
                RaisePropertyChangedEvent("Radius");
            }
        }

        public BindableVector Velocity => velocity;

        public double Mass
        {
            get => mass;
            set
            {
                mass = value;
                RaisePropertyChangedEvent("Mass");
            }
        }

        public double Charge
        {
            get => charge;
            set
            {
                charge = value;
                RaisePropertyChangedEvent("Charge");
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


        private Point _originalPosition;
        private Vector _originalVelocity;

        private void StoreProperty()
        {
            _originalPosition = Position;
            _originalVelocity = Velocity;
        }

        private void RecoverProperty()
        {
            Position.Set(_originalPosition);
            Velocity.Set(_originalVelocity);
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
