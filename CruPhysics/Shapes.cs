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

namespace CruPhysics.Shapes
{
    /// <summary>
    /// A Point class used for data-binding.
    /// </summary>
    public class BindablePoint : NotifyPropertyChangedObject
    {
        private double x = 0.0;
        private double y = 0.0;

        public BindablePoint()
        {

        }

        public BindablePoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public BindablePoint(Point point)
        {
            x = point.X;
            y = point.Y;
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

        public void Set(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        public void Move(Vector vector)
        {
            X += vector.X;
            Y += vector.Y;
        }

        public static explicit operator Point(BindablePoint point)
        {
            return new Point(point.x, point.y);
        }

        public static explicit operator BindablePoint(Point point)
        {
            return new BindablePoint(point);
        }
    }

    public class ShapeMouseEventArgs : EventArgs
    {
        private CruShape shape;
        private MouseEventArgs origin;

        public ShapeMouseEventArgs(CruShape shape, MouseEventArgs origin)
        {
            this.shape = shape;
            this.origin = origin;
        }

        public CruShape Shape
        {
            get
            {
                return shape;
            }
        }

        public MouseEventArgs Raw
        {
            get
            {
                return origin;
            }
        }

        public Point Position
        {
            get
            {
                var position = origin.GetPosition(shape.Canvas);
                return new Point(position.X, -position.Y);
            }
        }
    }

    public delegate void ShapeMouseEventHandler(object sender, ShapeMouseEventArgs e);

    /// <summary>
    /// <para>Represents a shape in a canvas.</para>
    /// <para>It preserves an internal shape object with cache of properties and provides some useful methods.</para>
    /// <para>Data binding should be bound through <see cref="Raw"/>.</para>
    /// </summary>
    public abstract class CruShape : NotifyPropertyChangedObject
    {
        private DispatcherOperation updateOperation;

        protected CruShape()
        {

        }

        protected void Initialize(Shape shape)
        {
            shape.MouseDown += (sender, e) => this.mouseDown?.Invoke(this, new ShapeMouseEventArgs(this, e));
            shape.MouseUp += (sender, e) => this.mouseUp?.Invoke(this, new ShapeMouseEventArgs(this, e));
            shape.MouseEnter += (sender, e) => this.mouseEnter?.Invoke(this, new ShapeMouseEventArgs(this, e));
            shape.MouseLeave += (sender, e) => this.mouseLeave?.Invoke(this, new ShapeMouseEventArgs(this, e));
            shape.MouseMove += (sender, e) => this.mouseMove?.Invoke(this, new ShapeMouseEventArgs(this, e));
        }

        protected abstract void DoUpdate();

        public void Update()
        {
            if (updateOperation != null)
                updateOperation.Abort();
            updateOperation = Raw.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(DoUpdate));
        }

        public void ForceUpdate()
        {
            DoUpdate();
        }

        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);
        public abstract Shape GetRawShape();

        internal abstract void ShowProperty(ShapePropertyControl shapePropertyControl);

        public abstract SelectionBox CreateSelectionBox();

        public Shape Raw
        {
            get
            {
                return GetRawShape();
            }
        }

        public Canvas Canvas
        {
            get
            {
                return GetRawShape().Parent as Canvas;
            }

            set
            {
                var parent = Canvas;
                if (parent == value)
                    return;

                if (parent != null)
                    parent.Children.Remove(GetRawShape());
                if (value != null)
                    value.Children.Add(GetRawShape());
            }
        }

        public void Delete()
        {
            Canvas = null;
        }

        public int ZIndex
        {
            get
            {
                return Canvas.GetZIndex(Raw);
            }

            set
            {
                Canvas.SetZIndex(Raw, value);
            }
        }


        public Brush Fill
        {
            get
            {
                return GetRawShape().Fill;
            }

            set
            {
                GetRawShape().Fill = value;
            }
        }

        public Brush Stroke
        {
            get
            {
                return GetRawShape().Stroke;
            }

            set
            {
                GetRawShape().Stroke = value;
            }
        }

        public double StrokeThickness
        {
            get
            {
                return GetRawShape().StrokeThickness;
            }

            set
            {
                GetRawShape().StrokeThickness = value;
            }
        }

        public ContextMenu ContextMenu
        {
            get
            {
                return GetRawShape().ContextMenu;
            }

            set
            {
                GetRawShape().ContextMenu = value;
            }
        }

        public Cursor Cursor
        {
            get
            {
                return GetRawShape().Cursor;
            }

            set
            {
                GetRawShape().Cursor = value;
            }
        }

        private ShapeMouseEventHandler mouseDown;
        private ShapeMouseEventHandler mouseUp;
        private ShapeMouseEventHandler mouseEnter;
        private ShapeMouseEventHandler mouseLeave;
        private ShapeMouseEventHandler mouseMove;


        public event ShapeMouseEventHandler MouseDown
        {
            add
            {
                mouseDown += value;
            }

            remove
            {
                mouseDown -= value;
            }
        }

        public event ShapeMouseEventHandler MouseUp
        {
            add
            {
                mouseUp += value;
            }

            remove
            {
                mouseUp -= value;
            }
        }

        public event ShapeMouseEventHandler MouseEnter
        {
            add
            {
                mouseEnter += value;
            }

            remove
            {
                mouseEnter -= value;
            }
        }

        public event ShapeMouseEventHandler MouseLeave
        {
            add
            {
                mouseLeave += value;
            }

            remove
            {
                mouseLeave -= value;
            }
        }

        public event ShapeMouseEventHandler MouseMove
        {
            add
            {
                mouseMove += value;
            }

            remove
            {
                mouseMove -= value;
            }
        }
    }

    public sealed class CruLine : CruShape
    {
        private Line shape = new Line();
        private BindablePoint point1 = new BindablePoint();
        private BindablePoint point2 = new BindablePoint();

        public CruLine()
        {
            Initialize(shape);

            point1.PropertyChanged += (sender, args) => Update();
            point2.PropertyChanged += (sender, args) => Update();

            Update();
        }

        public BindablePoint Point1
        {
            get
            {
                return point1;
            }
        }

        public BindablePoint Point2
        {
            get
            {
                return point2;
            }
        }

        public override Shape GetRawShape()
        {
            return shape;
        }

        public override SelectionBox CreateSelectionBox()
        {
            throw new NotImplementedException();
        }

        protected override void DoUpdate()
        {
            shape.X1 = point1.X;
            shape.Y1 = -point1.Y;
            shape.X2 = point2.X;
            shape.Y2 = -point2.Y;
        }

        public override bool IsPointInside(Point point)
        {
            System.Diagnostics.Debug.WriteLine("Try to test if a point is in a line.");
            return false;
        }

        public override void Move(Vector vector)
        {
            point1.Move(vector);
            point2.Move(vector);
        }

        public void Set(Point point1, Point point2)
        {
            this.point1.Set(point1);
            this.point2.Set(point2);
        }

        internal override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            throw new Exception("ShapePropertyControl can't show property of a line.");
        }
    }

    public sealed class Circle : CruShape
    {
        private Ellipse _shape = new Ellipse();
        private BindablePoint _center = new BindablePoint();
        private double _radius = 10.0;

        public Circle()
        {
            Initialize(_shape);

            _center.PropertyChanged += (sender, args) => Update();

            Update();
        }

        public override Shape GetRawShape()
        {
            return _shape;
        }

        public override SelectionBox CreateSelectionBox()
        {
            return new CircleSelectionBox(this);
        }
        
        protected override void DoUpdate()
        {
            _shape.Width = _radius * 2.0;
            _shape.Height = _radius * 2.0;
            Canvas.SetLeft(_shape, _center.X - _radius);
            Canvas.SetTop(_shape, -_center.Y - _radius);
        }

        internal override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            shapePropertyControl.circleRadioButton.IsChecked = true;
            shapePropertyControl.circleGrid.Visibility = Visibility.Visible;
            shapePropertyControl.centerXTextBox.Text = Center.X.ToString();
            shapePropertyControl.centerYTextBox.Text = Center.Y.ToString();
            shapePropertyControl.radiusTextBox.Text = Radius.ToString();
        }


        public BindablePoint Center
        {
            get
            {
                return _center;
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
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException
                        ("Radius", value, "Radius can't be smaller than 0.");

                _radius = value;
                Update();
                RaisePropertyChangedEvent("Radius");
            }
        }
        
        public override void Move(Vector vector)
        {
            Center.Move(vector);
        }

        public void Set(Point center, double radius)
        {
            Center.Set(center);
            Radius = radius;
        }

        public override bool IsPointInside(Point point)
        {
            var center = Center;
            return Math.Pow(point.X - center.X, 2) +
                Math.Pow(point.Y - center.Y, 2) < Math.Pow(Radius, 2);
        }
    }

    public sealed class Rectangle : CruShape
    {
        private System.Windows.Shapes.Rectangle _shape = new System.Windows.Shapes.Rectangle();
        private double _left = -50.0;
        private double _top = 50.0;
        private double _right = 50.0;
        private double _bottom = -50.0;

        public Rectangle()
        {
            Initialize(_shape);
            Update();

            PropertyChanged += (sender, args) => 
            {
                if (args.PropertyName == "Left")
                {
                    RaisePropertyChangedEvent("Width");
                    RaisePropertyChangedEvent("LeftTop");
                    RaisePropertyChangedEvent("LeftBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Top")
                {
                    RaisePropertyChangedEvent("Height");
                    RaisePropertyChangedEvent("LeftTop");
                    RaisePropertyChangedEvent("RightTop");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Right")
                {
                    RaisePropertyChangedEvent("Width");
                    RaisePropertyChangedEvent("RightTop");
                    RaisePropertyChangedEvent("RightBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Bottom")
                {
                    RaisePropertyChangedEvent("Height");
                    RaisePropertyChangedEvent("LeftBottom");
                    RaisePropertyChangedEvent("RightBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }
            };
        }

        public override SelectionBox CreateSelectionBox()
        {
            return new RectangleSelectionBox(this);
        }

        protected override void DoUpdate()
        {
            _shape.Width = _right - _left;
            _shape.Height = _top - _bottom;
            Canvas.SetLeft(_shape, _left);
            Canvas.SetTop(_shape, -_top);
        }

        internal override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            shapePropertyControl.rectangleRadioButton.IsChecked = true;
            shapePropertyControl.rectangleGrid.Visibility = Visibility.Visible;
            shapePropertyControl.leftTextBox.Text = Left.ToString();
            shapePropertyControl.topTextBox.Text = Top.ToString();
            shapePropertyControl.rightTextBox.Text = Right.ToString();
            shapePropertyControl.bottomTextBox.Text = Bottom.ToString();
        }

        public override Shape GetRawShape()
        {
            return _shape;
        }

        public Point Center
        {
            get
            {
                return new Point((Left + Right) / 2.0, (Top + Bottom) / 2.0);
            }
            set
            {
                var halfWidth = Width / 2.0;
                var halfHeight = Height / 2.0;
                Set(value.X - halfWidth,
                    value.Y + halfHeight,
                    value.X + halfWidth,
                    value.Y - halfHeight);
            }
        }

        public double Width
        {
            get
            {
                return Right - Left;
            }
        }

        public double Height
        {
            get
            {
                return Top - Bottom;
            }
        }

        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                if (value > Right)
                    throw new ArgumentOutOfRangeException
                        ("Left", value, "Left can't be bigger than Right.");
                _left = value;
                Update();
                RaisePropertyChangedEvent("Left");
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
                if (value < Bottom)
                    throw new ArgumentOutOfRangeException
                        ("Top", value, "Top can't be smaller than Bottom.");
                _top = value;
                Update();
                RaisePropertyChangedEvent("Top");
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
                if (value < Left)
                    throw new ArgumentOutOfRangeException
                        ("Right", value, "Right can't be smaller than Left.");
                _right = value;
                Update();
                RaisePropertyChangedEvent("Right");
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
                if (value > Top)
                    throw new ArgumentOutOfRangeException
                        ("Bottom", value, "Bottom can't be bigger than Top.");
                _bottom = value;
                Update();
                RaisePropertyChangedEvent("Bottom");
            }
        }

        public Point Lefttop
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        public Point Righttop
        {
            get
            {
                return new Point(Right, Top);
            }
        }

        public Point Leftbottom
        {
            get
            {
                return new Point(Left, Bottom);
            }
        }

        public Point Rightbottom
        {
            get
            {
                return new Point(Right, Bottom);
            }
        }

        private void NotifyAllPropertiesChanged()
        {
            RaisePropertyChangedEvent("Left");
            RaisePropertyChangedEvent("Top");
            RaisePropertyChangedEvent("Right");
            RaisePropertyChangedEvent("Bottom");
        }

        public void Set(double left, double top, double right, double bottom)
        {
            if (left > right || bottom > top)
                throw new ArgumentException();

            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;

            Update();
            NotifyAllPropertiesChanged();
        }

        public override void Move(Vector vector)
        {
            _left += vector.X;
            _right += vector.X;
            _top += vector.Y;
            _bottom += vector.Y;

            Update();
            NotifyAllPropertiesChanged();
        }

        public override bool IsPointInside(Point point)
        {
            return
                point.X > _left &&
                point.X < _right &&
                point.Y > _bottom &&
                point.Y < _top;
        }
    }
}
