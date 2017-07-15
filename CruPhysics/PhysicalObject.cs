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
using System.Windows.Shapes;

namespace CruPhysics
{
    public enum SelectionState
    {
        Normal,
        Hover,
        Select
    }

    public class Force : BindableVector
    {
        public Force()
        {

        }

        public Force(double x, double y)
            : base(x, y)
        {

        }

        public Force(Vector vector)
            : base(vector)
        {

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


        private Scene scene = null;
        private string name = string.Empty;
        private SelectionState selectionState = SelectionState.Normal;
        private Color color;

        public PhysicalObject()
        {
            Color = Common.GetRamdomColor();
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
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => RelatedScene));
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
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Name));
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
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => SelectionState));
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Color));
            }
        }

        public abstract int DefaultZIndex { get; }
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();


        protected void PrepareShape(Shape shape)
        {
            shape.Cursor = Cursors.Arrow;
            shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");

            SetShowState(shape, SelectionState);
        }

        private void SetShowState(Shape shape, SelectionState selectionState)
        {
            shape.Stroke = StrokeBrushes[SelectionState];
            shape.StrokeThickness = StrokeThickness[SelectionState];
            if (selectionState == SelectionState.Select)
                Panel.SetZIndex(shape, PhysicalObjectZIndex.Selected);
            else
                Panel.SetZIndex(shape, DefaultZIndex);
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
            RelatedScene = scene;
            scene.physicalObjects.Add(this);
        }

        protected virtual void OnRemoveFromScene(Scene scene)
        {
            RelatedScene = null;
            scene.physicalObjects.Remove(this);
        }
    }


    public class MovingObject : PhysicalObject
    {
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

        public override int DefaultZIndex => PhysicalObjectZIndex.MovingObject;

        public BindablePoint Position => position;

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
            _originalPosition = Position;
            _originalVelocity = Velocity;
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
        private CruShape shape = new CruRectangle();

        protected Field()
        {

        }
        
        public CruShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Shape));
            }
        }

        public override void Move(Vector vector)
        {
            Shape.Move(vector);
        }

        public bool IsMovingObjectInside(MovingObject movingObject)
        {
            return Shape.IsPointInside(movingObject.Position);
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (IsMovingObjectInside(movingObject))
                CalculateEffect(movingObject, time);
        }

        public abstract void CalculateEffect(MovingObject movingObject, TimeSpan time);

        public override int DefaultZIndex => PhysicalObjectZIndex.Field;

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
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => FluxDensity));
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
