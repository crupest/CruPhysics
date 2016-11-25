using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CruPhysics
{
    public abstract class CPShape
    {
        private bool _autoUpdate;

        public CPShape()
        {
            _autoUpdate = false;
        }

        public Shape Raw
        {
            get
            {
                return GetRawShape();
            }
        }

        public abstract void Update();
        protected void TryUpdate()
        {
            if (AutoUpdate)
                Update();
        }

        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);

        public abstract void ShowProperty(ShapePropertyControl shapePropertyControl);

        protected abstract Shape GetRawShape();
        
        public Canvas Canvas
        {
            get
            {
                return Raw.Parent as Canvas;
            }
            set
            {
                if (Canvas != null)
                    Canvas.Children.Remove(Raw);
                if (value != null)
                    value.Children.Add(Raw);
            }
        }

        public bool AutoUpdate
        {
            get
            {
                return _autoUpdate;
            }
            set
            {
                _autoUpdate = value;
            }
        }
    }

    public class CPCircle : CPShape
    {
        private Ellipse _shape  = new Ellipse();
        private Point   _center = new Point();
        private double  _radius = 10.0;

        public CPCircle()
        {
            Update();
        }

        protected override Shape GetRawShape()
        {
            return _shape;
        }

        public override void Update()
        {
            _shape.Width = _radius * 2.0;
            _shape.Height = _radius * 2.0;
            Canvas.SetLeft(_shape, _center.X - _radius);
            Canvas.SetTop(_shape, -_center.Y - _radius);
        }

        public override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            shapePropertyControl.circleRadioButton.IsChecked = true;
            shapePropertyControl.circleGrid.Visibility = Visibility.Visible;
            shapePropertyControl.centerXTextBox.Text = Center.X.ToString();
            shapePropertyControl.centerYTextBox.Text = Center.Y.ToString();
            shapePropertyControl.radiusTextBox.Text = Radius.ToString();
        }

        public Point Center
        {
            get
            {
                return _center;
            }
            set
            {
                _center = value;
                TryUpdate();
            }
        }

        public double Radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
                TryUpdate();
            }
        }

        public override void Move(Vector vector)
        {
            Center += vector;
        }

        public override bool IsPointInside(Point point)
        {
            var center = Center;
            return Math.Pow(point.X - center.X, 2) +
                Math.Pow(point.Y - center.Y, 2) < Math.Pow(Radius, 2);
        }
    }

    public class CPRectangle : CPShape
    {
        private Rectangle _shape  = new Rectangle();
        private double    _left   = -50.0;
        private double    _top    =  50.0;
        private double    _right  =  50.0;
        private double    _bottom = -50.0;

        public CPRectangle()
        {
            Update();
        }

        public override void Update()
        {
            _shape.Width = _right - _left;
            _shape.Height = _top - _bottom;
            Canvas.SetLeft(_shape, _left);
            Canvas.SetTop(_shape, -_top);
        }

        public override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            shapePropertyControl.rectangleRadioButton.IsChecked = true;
            shapePropertyControl.rectangleGrid.Visibility = Visibility.Visible;
            shapePropertyControl.leftTextBox  .Text = Left  .ToString();
            shapePropertyControl.topTextBox   .Text = Top   .ToString();
            shapePropertyControl.rightTextBox .Text = Right .ToString();
            shapePropertyControl.bottomTextBox.Text = Bottom.ToString();
        }

        protected override Shape GetRawShape()
        {
            return _shape;
        }
        
        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                _left = value;
                TryUpdate();
            }
        }

        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                _top = value;
                TryUpdate();
            }
        }

        public double Right
        {
            get
            {
                return _right;
            }
            set
            {
                _right = value;
                TryUpdate();
            }
        }

        public double Bottom
        {
            get
            {
                return _bottom;
            }
            set
            {
                _bottom = value;
                TryUpdate();
            }
        }

        public override void Move(Vector vector)
        {
            _left   += vector.X;
            _right  += vector.X;
            _top    += vector.Y;
            _bottom += vector.Y;

            TryUpdate();
        }

        public override bool IsPointInside(Point point)
        {
            return
                point.X > _left   &&
                point.X < _right  &&
                point.Y > _bottom &&
                point.Y < _top;
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
        public static int selected = int.MaxValue;
        public static int movingObject = 2;
        public static int field = 1;
    }

    public abstract class PhysicalObject
    {
        private static PhysicalObject selectedObject = null;

        public static PhysicalObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject != null)
                {
                    selectedObject.SetShowState(SelectionState.normal);
                }
                selectedObject = value;
                if (value != null)
                {
                    value.SetShowState(SelectionState.select);
                }
            }
        }

        private static Point cursorPreviousPosition;




        public PhysicalObject()
        {

        }

        public string Name { get; set; }

        public CPShape Shape
        {
            get
            {
                return GetShape();
            }
        }


        protected abstract CPShape GetShape();
        public abstract Brush FillBrush { get; }
        public abstract int DefaultZIndex { get; }


        /// <summary>
        /// 一个CPShape对象作为PhysicalObject的形状之前所进行的一般处理。
        /// </summary>
        protected void PrepareShape()
        {
            Shape.Canvas = RelatedScene == null ?
                    null : RelatedScene.RelatedMainWindow.worldCanvas;
            Shape.Raw.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");
            Shape.Raw.MouseEnter += PhysicalObject_OnMouseEnter;
            Shape.Raw.MouseLeave += PhysicalObject_OnMouseLeave;
            Shape.Raw.MouseDown  += PhysicalObject_OnMouseDown;
            Shape.Raw.MouseUp    += PhysicalObject_OnMouseUp;
            Shape.Raw.MouseMove  += PhysicalObject_OnMouseMove;

            SetShowState(GetSelectionState());

            Shape.Raw.Fill = FillBrush;
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

        private void SetShowState(SelectionState selectionState)
        {
            switch (selectionState)
            {
                case SelectionState.normal:
                    Shape.Raw.Stroke = normal_stroke;
                    Shape.Raw.StrokeThickness = 1.0;
                    Canvas.SetZIndex(Shape.Raw, DefaultZIndex);
                    break;
                case SelectionState.hover:
                    Shape.Raw.Stroke = hover_stroke;
                    Shape.Raw.StrokeThickness = 2.0;
                    Canvas.SetZIndex(Shape.Raw, DefaultZIndex);
                    break;
                case SelectionState.select:
                    Shape.Raw.Stroke = select_stroke;
                    Shape.Raw.StrokeThickness = 2.0;
                    Canvas.SetZIndex(Shape.Raw, PhysicalObjectZIndex.selected);
                    break;
            }
        }


        internal abstract Window CreatePropertyWindow();

        protected virtual void AddToScene(Scene scene)
        {
            scene.RelatedWorldCanvas.Children.Add(Shape.Raw);
            scene.physicalObjects.Add(this);
        }

        protected virtual void RemoveFromScene(Scene scene)
        {
            scene.RelatedWorldCanvas.Children.Remove(Shape.Raw);
            scene.physicalObjects.Remove(this);
        }

        private void PhysicalObject_OnMouseEnter(object sender, MouseEventArgs args)
        {
            if (SelectedObject == this)
                return;
            SetShowState(SelectionState.hover);
        }


        private void PhysicalObject_OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (SelectedObject == this)
                return;
            SetShowState(SelectionState.normal);
        }


        private void PhysicalObject_OnMouseDown(object sender, MouseButtonEventArgs args)
        {
            SelectedObject = this;
            Mouse.Capture(Shape.Raw);
            args.Handled = true;
        }


        private void PhysicalObject_OnMouseUp(object sender, MouseButtonEventArgs args)
        {
            Mouse.Capture(null);
        }


        private void PhysicalObject_OnMouseMove(object sender, MouseEventArgs args)
        {
            var newPosition = args.GetPosition(RelatedScene.RelatedMainWindow.worldCanvas);
            if (Shape.Raw.IsMouseCaptured)
            {
                var displacement = newPosition - cursorPreviousPosition;
                displacement.Y = -displacement.Y;
                Move(displacement);
                Update();
            }
            cursorPreviousPosition = newPosition;
        }
    }



    public class MovingObject : PhysicalObject
    {
        private CPCircle _shape = new CPCircle();

        public MovingObject()
        {
            PrepareShape();
            Mass = 1.0;
        }

        protected override CPShape GetShape()
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
        private CPShape _shape = new CPRectangle(); 

        public Field()
        {
            PrepareShape();
        }

        public void SetShape(CPShape shape)
        {
            _shape.Canvas = null;
            _shape = shape;
            PrepareShape();
        }

        protected override CPShape GetShape()
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

        private static readonly string timeFormat = @"mm\:ss\.ff";


        private readonly MainWindow _mainWindow;

        internal List<PhysicalObject>    physicalObjects = new List<PhysicalObject>();
        internal List<MovingObject>      movingObjects = new List<MovingObject>();
        internal List<Field>             fields = new List<Field>();

        private DispatcherTimer timer = new DispatcherTimer();
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
