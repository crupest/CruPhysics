﻿using System;
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

        public static explicit operator BindableVector(Vector vector)
        {
            return new BindableVector(vector);
        }
    }

    public enum SelectionState
    {
        normal,
        hover,
        select
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
        private string name = string.Empty;
        private SolidColorBrush fill = new SolidColorBrush();

        public PhysicalObject()
        {
            OnCreateFillBrush(fill);
            Color = Common.GetRamdomColor();
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

        public CruShape Shape
        {
            get
            {
                return GetShape();
            }
        }

        public Color Color
        {
            get
            {
                return fill.Color;
            }
            set
            {
                fill.Color = value;
                RaisePropertyChangedEvent("Color");
            }
        }

        protected virtual void OnCreateFillBrush(SolidColorBrush brush)
        {

        }

        protected abstract CruShape GetShape();
        public abstract int DefaultZIndex { get; }

        protected void PrepareShape()
        {
            Shape.Canvas = RelatedScene?.RelatedWorldCanvas;
            Shape.Cursor = Cursors.Arrow;
            Shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");
            Shape.MouseEnter += PhysicalObject_OnMouseEnter;
            Shape.MouseLeave += PhysicalObject_OnMouseLeave;
            Shape.MouseDown  += PhysicalObject_OnMouseDown;

            SetShowState(GetSelectionState());

            Shape.Fill = fill;
        }

        private Scene _scene;

        public Scene RelatedScene
        {
            get
            {
                return _scene;
            }

        }

        public void Move(Vector vector)
        {
            Shape.Move(vector);
        }

        private static readonly Brush normal_stroke = Brushes.Black;
        private static readonly Brush hover_stroke  = Brushes.Red;
        private static readonly Brush select_stroke = Brushes.Blue;
        
        public bool IsSelected
        {
            get
            {
                if (RelatedScene == null)
                    return false;
                return RelatedScene.SelectedObject == this; 
            }
        }

        public SelectionState GetSelectionState()
        {
            if (RelatedScene == null)
                return SelectionState.normal;

            if (RelatedScene.SelectedObject == this)
                return SelectionState.select;
            else if (Shape.Raw.IsMouseDirectlyOver == true)
                return SelectionState.hover;
            else
                return SelectionState.normal;
        }

        internal void SetShowState(SelectionState selectionState)
        {
            switch (selectionState)
            {
                case SelectionState.normal:
                    Shape.Stroke = normal_stroke;
                    Shape.StrokeThickness = 1.0;
                    Shape.ZIndex = DefaultZIndex;
                    break;
                case SelectionState.hover:
                    Shape.Stroke = hover_stroke;
                    Shape.StrokeThickness = 2.0;
                    Shape.ZIndex = DefaultZIndex;
                    break;
                case SelectionState.select:
                    Shape.Stroke = select_stroke;
                    Shape.StrokeThickness = 2.0;
                    Shape.ZIndex = PhysicalObjectZIndex.Selected;
                    break;
            }
        }

        internal void FinishCalculate(Scene scene)
        {
            OnCalculated(scene);
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

        protected virtual void OnCalculated(Scene scene)
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

        internal abstract Window CreatePropertyWindow();

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
            _scene = scene;
            Shape.Canvas = scene.RelatedWorldCanvas;
            scene.physicalObjects.Add(this);
        }

        protected virtual void OnRemoveFromScene(Scene scene)
        {
            _scene = null;

            if (IsSelected)
                RelatedScene.SelectedObject = null;

            Shape.Canvas = null;
            scene.physicalObjects.Remove(this);
        }

        private void PhysicalObject_OnMouseEnter(object sender, ShapeMouseEventArgs args)
        {
            if (IsSelected)
                return;
            SetShowState(SelectionState.hover);
        }


        private void PhysicalObject_OnMouseLeave(object sender, ShapeMouseEventArgs args)
        {
            if (IsSelected)
                return;
            SetShowState(SelectionState.normal);
        }


        private void PhysicalObject_OnMouseDown(object sender, ShapeMouseEventArgs args)
        {
            RelatedScene.SelectedObject = this;
            args.Raw.Handled = true;
        }
    }



    public class MovingObject : PhysicalObject
    {
        private CruCircle _shape = new CruCircle();
        private MotionTrail trail = new MotionTrail();

        public MovingObject()
        {
            Name = "运动对象";
            PrepareShape();

            _shape.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Radius")
                    RaisePropertyChangedEvent("Radius");
            };

            Mass = 1.0;
        }

        protected override CruShape GetShape()
        {
            return _shape;
        }

        public override int DefaultZIndex
        {
            get
            {
                return PhysicalObjectZIndex.MovingObject;
            }
        }

        public BindablePoint Position
        {
            get
            {
                return _shape.Center;
            }
        }

        public double Radius
        {
            get
            {
                return _shape.Radius;
            }
            set
            {
                _shape.Radius = value;
            }
        }

        private List<Force> forces = new List<Force>();

        public void AddForce(Force force)
        {
            forces.Add(force);
        }

        public void ClearForce()
        {
            forces.Clear();
        }

        private BindableVector velocity = new BindableVector();
        private double mass;
        private double charge;

        public BindableVector Velocity
        {
            get
            {
                return velocity;
            }
        }

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

        internal override Window CreatePropertyWindow()
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

        protected override void OnCalculated(Scene scene)
        {
            base.OnCalculated(scene);
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
            PrepareShape();
        }

        public new CruShape Shape
        {
            get
            {
                return GetShape();
            }
            set
            {
                SetShape(value);
            }
        }

        public void SetShape(CruShape shape)
        {
            _shape.Canvas = null;
            _shape = shape;
            PrepareShape();
            RaisePropertyChangedEvent("Shape");
        }

        protected override void OnCreateFillBrush(SolidColorBrush brush)
        {
            base.OnCreateFillBrush(brush);
            brush.Opacity = 0.3;
        }

        protected override CruShape GetShape()
        {
            return _shape;
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (Shape.IsPointInside((Point)movingObject.Position))
                CalculateEffect(movingObject, time);
        }

        public override int DefaultZIndex
        {
            get
            {
                return PhysicalObjectZIndex.Field;
            }
        }

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

        internal override Window CreatePropertyWindow()
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

        internal override Window CreatePropertyWindow()
        {
            return new MagneticFieldPropertyDialog(this);
        }
    }
    


    public class Scene : NotifyPropertyChangedObject
    {
        private static Scene currentScene;

        public static Scene Current
        {
            get
            {
                return currentScene;
            }
            set
            {
                currentScene = value;
            }
        }

        private const string timeFormat = @"mm\:ss\.ff";


        private readonly MainWindow _mainWindow;

        internal ObservableCollection<PhysicalObject> physicalObjects = new ObservableCollection<PhysicalObject>();
        internal ObservableCollection<MovingObject> movingObjects = new ObservableCollection<MovingObject>();
        internal ObservableCollection<Field> fields = new ObservableCollection<Field>();

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun = false;


        private PhysicalObject selectedObject = null;
        private SelectionBox selectionBox = null;

        private void CreateSelectionBox()
        {
            selectionBox = selectedObject.Shape.CreateSelectionBox();
            selectionBox.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");
        }

        public PhysicalObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject == value)
                    return;

                if (selectedObject != null)
                {
                    selectedObject.SetShowState(SelectionState.normal);
                    selectionBox.Delete();
                }
                selectedObject = value;
                if (value != null)
                {
                    value.SetShowState(SelectionState.select);
                    CreateSelectionBox();
                }

                RaisePropertyChangedEvent("SelectedObject");
            }
        }

        public Scene(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            Current = this;
            ScanInterval = TimeSpan.FromMilliseconds(15.0);
            timer.Tick += Run;
        }

        public ObservableCollection<PhysicalObject> PhysicalObjects
        {
            get
            {
                return physicalObjects;
            }
        }

        private void Run(object sender, EventArgs e)
        {
            runningTime += ScanInterval;
            RelatedMainWindow.TimeTextBox.Text = runningTime.ToString(timeFormat);

            var calculationInterval = ScanInterval;
            foreach (var i in movingObjects)
            {
                foreach (var j in fields)
                    j.Influence(i, calculationInterval);
                i.Run(calculationInterval);
                i.ClearForce();
            }

            foreach (var i in physicalObjects)
            {
                i.FinishCalculate(this);
            }
        }

        public MainWindow RelatedMainWindow
        {
            get
            {
                return _mainWindow;
            }
        }

        public Canvas RelatedWorldCanvas
        {
            get
            {
                return RelatedMainWindow.WorldCanvas;
            }
        }

        private void PhysicalObjectShapeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shape" && ((PhysicalObject)sender).IsSelected)
            {
                selectionBox.Delete();
                CreateSelectionBox();
            }
        }

        public void Add(PhysicalObject physicalObject)
        {
            physicalObject.AddToScene(this);

            if (physicalObject is Field)
                physicalObject.PropertyChanged += PhysicalObjectShapeChanged;
        }

        public void Remove(PhysicalObject physicalObject)
        {
            physicalObject.RemoveFromScene(this);

            if (physicalObject is Field)
                physicalObject.PropertyChanged -= PhysicalObjectShapeChanged;
        }

        public double Bounds
        {
            get
            {
                return 1000.0;
            }
        }

        public TimeSpan ScanInterval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return timer.IsEnabled;
            }
        }

        /// <summary>
        /// <see cref="HasBegun"/> will be true even if it is paused.
        /// </summary>
        public bool HasBegun
        {
            get
            {
                return hasBegun;
            }
        }

        public TimeSpan RunningTime
        {
            get
            {
                return runningTime;
            }
        }

        public void Begin()
        {
            if (!hasBegun)
            {
                foreach (var i in physicalObjects)
                {
                    i.BeginRunning(this);
                }
                hasBegun = true;
            }
            RelatedMainWindow.TimeTextBox.Visibility = Visibility.Visible;
            timer.Start();
        }
        
        public void Stop()
        {
            timer.Stop();
            foreach (var i in physicalObjects)
            {
                i.StopRunning(this);
            }
        }

        public void Restart()
        {
            if (IsRunning)
                Stop();
            hasBegun = false;
            runningTime = TimeSpan.Zero;
            foreach (var i in physicalObjects)
            {
                i.Refresh(this);
            }
            RelatedMainWindow.TimeTextBox.Visibility = Visibility.Collapsed;
            RelatedMainWindow.TimeTextBox.Text = runningTime.ToString(timeFormat);
        }
    }
}
