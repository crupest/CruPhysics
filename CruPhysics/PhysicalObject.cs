using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using CruPhysics.Shapes;

namespace CruPhysics
{
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
        public const int selected = 100;
        public const int controller = 101;
        public const int movingObject = 2;
        public const int field = 1;
    }

    public abstract class PhysicalObject
    {
        private static PhysicalObject selectedObject = null;
        private static SelectionBox selectionBox = null;

        public static PhysicalObject SelectedObject
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
                    selectionBox = selectedObject.Shape.CreateSelectionBox();
                }
            }
        }



        public PhysicalObject()
        {

        }

        public string Name { get; set; }

        public Shape Shape
        {
            get
            {
                return GetShape();
            }
        }


        protected abstract Shape GetShape();
        public abstract Brush FillBrush { get; }
        public abstract int DefaultZIndex { get; }

        protected void PrepareShape()
        {
            Shape.Canvas = RelatedScene?.RelatedWorldCanvas;
            Shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");
            Shape.MouseEnter += PhysicalObject_OnMouseEnter;
            Shape.MouseLeave += PhysicalObject_OnMouseLeave;
            Shape.MouseDown  += PhysicalObject_OnMouseDown;

            SetShowState(GetSelectionState());

            Shape.Fill = FillBrush;
        }

        private Scene _scene;

        public Scene RelatedScene
        {
            get
            {
                return _scene;
            }
            set
            {
                if (_scene != null)
                    RemoveFromScene(_scene);
                _scene = value;
                if (value != null)
                    AddToScene(value);
            }
        }

        public void Update()
        {
            Shape.Update();
        }

        public void Move(Vector vector)
        {
            Shape.Move(vector);
        }

        private static readonly Brush normal_stroke = Brushes.Black;
        private static readonly Brush hover_stroke  = Brushes.Red;
        private static readonly Brush select_stroke = Brushes.Blue;
        
        private SelectionState GetSelectionState()
        {
            if (SelectedObject == this)
                return SelectionState.select;
            else if (Shape.Raw.IsMouseDirectlyOver == true)
                return SelectionState.hover;
            else
                return SelectionState.normal;
        }

        protected virtual void SetShowState(SelectionState selectionState)
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
                    Shape.ZIndex = PhysicalObjectZIndex.selected;
                    break;
            }
        }


        internal abstract Window CreatePropertyWindow();

        protected virtual void AddToScene(Scene scene)
        {
            Shape.Canvas = scene.RelatedWorldCanvas;
            scene.physicalObjects.Add(this);
        }

        protected virtual void RemoveFromScene(Scene scene)
        {
            if (SelectedObject == this)
                SelectedObject = null;

            Shape.Canvas = null;
            scene.physicalObjects.Remove(this);
        }

        private void PhysicalObject_OnMouseEnter(object sender, ShapeMouseEventArgs args)
        {
            if (SelectedObject == this)
                return;
            SetShowState(SelectionState.hover);
        }


        private void PhysicalObject_OnMouseLeave(object sender, ShapeMouseEventArgs args)
        {
            if (SelectedObject == this)
                return;
            SetShowState(SelectionState.normal);
        }


        private void PhysicalObject_OnMouseDown(object sender, ShapeMouseEventArgs args)
        {
            SelectedObject = this;
            args.Raw.Handled = true;
        }
    }



    public class MovingObject : PhysicalObject
    {
        private Circle _shape = new Circle();

        public MovingObject()
        {
            PrepareShape();
            Mass = 1.0;
        }

        protected override Shape GetShape()
        {
            return _shape;
        }

        public override Brush FillBrush
        {
            get
            {
                return Brushes.SkyBlue;
            }
        }

        public override int DefaultZIndex
        {
            get
            {
                return PhysicalObjectZIndex.movingObject;
            }
        }

        public Point Position
        {
            get
            {
                return _shape.Center;
            }
            set
            {
                _shape.Center = value;
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

        public Vector Velocity  { get; set; }
        public double Mass      { get; set; }
        public double Charge    { get; set; }

        internal override Window CreatePropertyWindow()
        {
            return new MovingObjectPropertyDialog(this);
        }

        protected override void AddToScene(Scene scene)
        {
            base.AddToScene(scene);
            scene.movingObjects.Add(this);
        }

        protected override void RemoveFromScene(Scene scene)
        {
            base.RemoveFromScene(scene);
            scene.movingObjects.Remove(this);
        }

        private Point _originalPosition;
        private Vector _originalVelocity;

        public void StoreProperty()
        {
            _originalPosition = Position;
            _originalVelocity = Velocity;
        }

        public void RecoverProperty()
        {
            Position = _originalPosition;
            Velocity = _originalVelocity;
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
            Position += Velocity * t + acceleration * Math.Pow(t, 2) / 2.0;
            Velocity += acceleration * t;
        }
    }



    public abstract class Field : PhysicalObject
    {
        private Shape _shape = new Rectangle(); 

        public Field()
        {
            _shape.AutoUpdate = true;
            PrepareShape();
        }

        public void SetShape(Shape shape)
        {
            _shape.Canvas = null;
            _shape = shape;
            shape.AutoUpdate = true;
            PrepareShape();
        }

        protected override Shape GetShape()
        {
            return _shape;
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (Shape.IsPointInside(movingObject.Position))
                CalculateEffect(movingObject, time);
        }

        public override int DefaultZIndex
        {
            get
            {
                return PhysicalObjectZIndex.field;
            }
        }

        protected abstract void CalculateEffect(MovingObject movingObject, TimeSpan time);

        protected override void AddToScene(Scene scene)
        {
            base.AddToScene(scene);
            scene.fields.Add(this);
        }

        protected override void RemoveFromScene(Scene scene)
        {
            base.RemoveFromScene(scene);
            scene.fields.Remove(this);
        }
    }

    public class ElectricField : Field
    {
        static ElectricField()
        {
            fillBrush = new SolidColorBrush(Colors.Green);
            fillBrush.Opacity = 0.5;
        }

        private static readonly Brush fillBrush;

        public Vector Intensity { get; set; }

        public override Brush FillBrush
        {
            get
            {
                return fillBrush;
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
        static MagneticField()
        {
            fillBrush = new SolidColorBrush(Colors.Orange);
            fillBrush.Opacity = 0.5;
        }

        private static readonly Brush fillBrush;

        /// <summary>
        /// 为正时垂直纸面向里，为负时垂直于纸面向外
        /// </summary>
        public double FluxDensity { get; set; }

        public override Brush FillBrush
        {
            get
            {
                return fillBrush;
            }
        }

        protected override void CalculateEffect(MovingObject movingObject, TimeSpan time)
        {
            //qvB = mvω => ω = qB/m => α = ωt = qBt/m
            movingObject.Velocity = Common.Rotate(movingObject.Velocity,
                -time.TotalSeconds * movingObject.Charge * FluxDensity / movingObject.Mass);
        }

        internal override Window CreatePropertyWindow()
        {
            return new MagneticFieldPropertyDialog(this);
        }
    }
    


    public class Scene
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

        internal List<PhysicalObject>    physicalObjects = new List<PhysicalObject>();
        internal List<MovingObject>      movingObjects = new List<MovingObject>();
        internal List<Field>             fields = new List<Field>();

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun = false;

        public Scene(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            Current = this;
            ScanInterval = TimeSpan.FromMilliseconds(50.0);
            timer.Tick += Run;
        }

        private void Run(object sender, EventArgs e)
        {
            runningTime += ScanInterval;
            RelatedMainWindow.timeTextBox.Text = runningTime.ToString(timeFormat);

            var calculationInterval = TimeSpan.FromTicks(ScanInterval.Ticks / 1000);
            for (int k = 1; k <= 1000; k++)
                foreach (var i in movingObjects)
                {
                    foreach (var j in fields)
                        j.Influence(i, calculationInterval);
                    i.Run(calculationInterval);
                    i.ClearForce();
                }

            foreach (var i in movingObjects)
                i.Update();
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
                return RelatedMainWindow.worldCanvas;
            }
        }

        public void Add(PhysicalObject physicalObject)
        {
            physicalObject.RelatedScene = this;
        }

        public void Remove(PhysicalObject physicalObject)
        {
            physicalObject.RelatedScene = null;
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

        public bool HasBegun
        {
            get
            {
                return hasBegun;
            }
        }

        public void Begin()
        {
            if (!hasBegun)
            {
                foreach (var i in movingObjects)
                {
                    i.StoreProperty();
                }
                hasBegun = true;
            }
            RelatedMainWindow.timeTextBox.Visibility = Visibility.Visible;
            timer.Start();
        }
        
        public void Stop()
        {
            timer.Stop();
        }

        public void Restart()
        {
            if (IsRunning)
                Stop();
            hasBegun = false;
            foreach (var item in movingObjects)
            {
                item.RecoverProperty();
                item.Update();
            }
            runningTime = TimeSpan.Zero;
            RelatedMainWindow.timeTextBox.Visibility = Visibility.Collapsed;
            RelatedMainWindow.timeTextBox.Text = runningTime.ToString(timeFormat);
        }
    }
}
