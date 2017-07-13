using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using CruPhysics.Shapes;
using CruPhysics.Windows;
using CruPhysics.Controls;

namespace CruPhysics
{
    public class BindableVector : NotifyPropertyChangedObject
    {
        private double x = 0.0;
        private double y = 0.0;

        public BindableVector()
        {

        }

        public BindableVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public BindableVector(Vector vector)
        {
            x = vector.X;
            y = vector.Y;
        }

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                RaisePropertyChangedEvent("X");
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        public void Set(Vector vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public void Add(Vector vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static explicit operator Vector(BindableVector vector)
        {
            return new Vector(vector.x, vector.y);
        }
    }

    public enum SelectionState
    {
        Normal,
        Hover,
        Select
    }

    public struct Force
    {
        private double x;
        private double y;

        public Force(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
    }

    public static class PhysicalObjectZIndex
    {
        public const int Selected = 100;
        public const int Controller = 101;
        public const int MovingObject = 2;
        public const int Field = 1;
    }

    public abstract class PhysicalObject : NotifyPropertyChangedObject
    {
        static PhysicalObject()
        {
            strokeBrushes.Add(SelectionState.Normal, Brushes.Black);
            strokeBrushes.Add(SelectionState.Hover, Brushes.Red);
            strokeBrushes.Add(SelectionState.Select, Brushes.Blue);

            strokeThickness.Add(SelectionState.Normal, 1.0);
            strokeThickness.Add(SelectionState.Hover, 2.0);
            strokeThickness.Add(SelectionState.Select, 2.0);
        }

        private static readonly Dictionary<SelectionState, Brush> strokeBrushes = new Dictionary<SelectionState, Brush>();
        private static readonly Dictionary<SelectionState, double> strokeThickness = new Dictionary<SelectionState, double>(); 

        public static IReadOnlyDictionary<SelectionState, Brush> StrokeBrushes => strokeBrushes;
        public static IReadOnlyDictionary<SelectionState, double> StrokeThickness => strokeThickness;


        private Scene scene;
        private string name = string.Empty;
        private SelectionState selectionState = SelectionState.Normal;

        public PhysicalObject()
        {

        }

        public Scene RelatedScene
        {
            get
            {
                return scene;
            }
            private set
            {
                scene = value;
                RaisePropertyChangedEvent("RelatedScene");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChangedEvent("Name");
            }
        }

        public SelectionState SelectionState
        {
            get
            {
                return selectionState;
            }
            set
            {
                selectionState = value;
                RaisePropertyChangedEvent("SelectionState");
            }
        }

        public abstract Color Color { get; set; }
        public abstract int DefaultZIndex { get; }
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();


        protected void PrepareShape(CruShape shape)
        {
            shape.Canvas = RelatedScene?.RelatedWorldCanvas;
            shape.Cursor = Cursors.Arrow;
            shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");

            SetShowState(shape, SelectionState);
        }

        private void SetShowState(CruShape shape, SelectionState selectionState)
        {
            shape.Stroke = StrokeBrushes[SelectionState];
            shape.StrokeThickness = StrokeThickness[SelectionState];
            switch (selectionState)
            {
                case SelectionState.Normal:
                    shape.ZIndex = DefaultZIndex;
                    break;
                case SelectionState.Hover:
                    shape.ZIndex = DefaultZIndex;
                    break;
                case SelectionState.Select:
                    shape.ZIndex = PhysicalObjectZIndex.Selected;
                    break;
            }
        }


        internal void FinishOneScan(Scene scene)
        {
            OnOneScanned(scene);
        }

        internal void BeginRunning(Scene scene)
        {
            OnBeginRunning(scene);
        }

        internal void StopRunning(Scene scene)
        {
            OnStopRunning(scene);
        }

        internal void Refresh(Scene scene)
        {
            OnRefresh(scene);
        }

        protected virtual void OnOneScanned(Scene scene)
        {

        }

        protected virtual void OnBeginRunning(Scene scene)
        {

        }

        protected virtual void OnStopRunning(Scene scene)
        {

        }

        protected virtual void OnRefresh(Scene scene)
        {

        }

        internal void AddToScene(Scene scene)
        {
            OnAddToScene(scene);
        }

        internal void RemoveFromScene(Scene scene)
        {
            OnRemoveFromScene(scene);
        }

        protected virtual void OnAddToScene(Scene scene)
        {
            this.scene = scene;
            scene.physicalObjects.Add(this);
        }

        protected virtual void OnRemoveFromScene(Scene scene)
        {
            this.scene = null;
            scene.physicalObjects.Remove(this);
        }
    }


    public class MovingObject : PhysicalObject
    {
        private CruCircle shape = new CruCircle();
        private MotionTrail trail = new MotionTrail();

        private double radius;
        private BindableVector velocity = new BindableVector();
        private double mass;
        private double charge;

        private List<Force> forces = new List<Force>();

        public MovingObject()
        {
            Name = "运动对象";
            Radius = 10.0;
            Mass = 1.0;
        }

        public override int DefaultZIndex => PhysicalObjectZIndex.MovingObject;

        public BindablePoint Position
        {
            get
            {
                return shape.Center;
            }
        }

        public double Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                RaisePropertyChangedEvent("Radius");
            }
        }


        public void AddForce(Force force)
        {
            forces.Add(force);
        }

        public void ClearForce()
        {
            forces.Clear();
        }

        public BindableVector Velocity => velocity;

        public double Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
                RaisePropertyChangedEvent("Mass");
            }
        }

        public double Charge
        {
            get
            {
                return charge;
            }
            set
            {
                charge = value;
                RaisePropertyChangedEvent("Charge");
            }
        }

        public override Window CreatePropertyWindow()
        {
            return new MovingObjectPropertyDialog(this);
        }

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
            scene.movingObjects.Add(this);
            scene.RelatedWorldCanvas.Children.Add(trail.Shape);
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
            scene.movingObjects.Remove(this);
            scene.RelatedWorldCanvas.Children.Remove(trail.Shape);
        }

        protected override void OnBeginRunning(Scene scene)
        {
            base.OnBeginRunning(scene);
            StoreProperty();
            trail.AddPoint((Point)Position);
        }

        protected override void OnRefresh(Scene scene)
        {
            base.OnRefresh(scene);
            RecoverProperty();
            trail.Clear();
        }

        protected override void OnOneScanned(Scene scene)
        {
            base.OnOneScanned(scene);
            trail.AddPoint((Point)Position);
        }

        private Point _originalPosition;
        private Vector _originalVelocity;

        private void StoreProperty()
        {
            _originalPosition = (Point)Position;
            _originalVelocity = (Vector)Velocity;
        }

        private void RecoverProperty()
        {
            Position.Set(_originalPosition);
            Velocity.Set(_originalVelocity);
        }

        public void Run(TimeSpan time)
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



    public abstract class Field : PhysicalObject
    {
        private CruShape _shape = new CruRectangle(); 

        protected Field()
        {

        }

        public CruShape Shape
        {
            get
            {
            }
            set
            {
                _shape.Canvas = null;
                _shape = shape;
                PrepareShape();
                RaisePropertyChangedEvent("Shape");
            }
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (Shape.IsPointInside((Point)movingObject.Position))
                CalculateEffect(movingObject, time);
        }

        public override int DefaultZIndex => PhysicalObjectZIndex.Field;

        protected abstract void CalculateEffect(MovingObject movingObject, TimeSpan time);

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
            scene.fields.Add(this);
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
            scene.fields.Remove(this);
        }
    }

    public class ElectricField : Field
    {
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

        protected override void CalculateEffect(MovingObject movingObject, TimeSpan no_use)
        {
            movingObject.AddForce(
                new Force(Intensity.X * movingObject.Charge,
                    Intensity.Y * movingObject.Charge));
        }

        public override Window CreatePropertyWindow()
        {
            return new ElectricFieldPropertyDialog(this);
        }
    }

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
            get
            {
                return fluxDensity;
            }
            set
            {
                fluxDensity = value;
                RaisePropertyChangedEvent("FluxDensity");
            }
        }

        protected override void CalculateEffect(MovingObject movingObject, TimeSpan time)
        {
            //qvB = mvω => ω = qB/m => α = ωt = qBt/m
            movingObject.Velocity.Set(Common.Rotate((Vector)movingObject.Velocity,
                -time.TotalSeconds * movingObject.Charge * FluxDensity / movingObject.Mass));
        }

        public override Window CreatePropertyWindow()
        {
            return new MagneticFieldPropertyDialog(this);
        }
    }
}
