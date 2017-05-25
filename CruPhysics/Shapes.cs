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

namespace CruPhysics.Shapes
{
    /// <summary>
    /// Represents a shape in a canvas.
    /// It preserves an internal shape object with cache of properties and provides some useful methods.
    /// </summary>
    public abstract class Shape
    {
        private bool _autoUpdate = false;

        protected Shape()
        {

        }

        public virtual void Update()
        {
            updated?.Invoke(this, EventArgs.Empty);
        }

        protected void TryUpdate()
        {
            if (AutoUpdate)
                Update();
        }

        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);
        public abstract System.Windows.Shapes.Shape GetRawShape();
        internal abstract void ShowProperty(ShapePropertyControl shapePropertyControl);

        public abstract SelectionBox CreateSelectionBox();

        public System.Windows.Shapes.Shape Raw
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

        public delegate void ShapeUpdatedHandler(object sender, EventArgs e);
        private ShapeUpdatedHandler updated;
        public event ShapeUpdatedHandler Updated
        {
            add
            {
                updated += value;
            }

            remove
            {
                updated -= value;
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

        public event MouseButtonEventHandler MouseDown
        {
            add
            {
                GetRawShape().MouseDown += value;
            }

            remove
            {
                GetRawShape().MouseDown -= value;
            }
        }

        public event MouseButtonEventHandler MouseUp
        {
            add
            {
                GetRawShape().MouseUp += value;
            }

            remove
            {
                GetRawShape().MouseUp -= value;
            }
        }

        public event MouseEventHandler MouseEnter
        {
            add
            {
                GetRawShape().MouseEnter += value;
            }

            remove
            {
                GetRawShape().MouseEnter -= value;
            }
        }

        public event MouseEventHandler MouseLeave
        {
            add
            {
                GetRawShape().MouseLeave += value;
            }

            remove
            {
                GetRawShape().MouseLeave -= value;
            }
        }

        public event MouseEventHandler MouseMove
        {
            add
            {
                GetRawShape().MouseMove += value;
            }

            remove
            {
                GetRawShape().MouseMove -= value;
            }
        }
    }

    public sealed class Line : Shape
    {
        private System.Windows.Shapes.Line shape = new System.Windows.Shapes.Line();
        private Point point1 = new Point();
        private Point point2 = new Point();

        public Line()
        {
            Update();
        }

        public Point Point1
        {
            get
            {
                return point1;
            }

            set
            {
                point1 = value;
                TryUpdate();
            }
        }

        public Point Point2
        {
            get
            {
                return point2;
            }

            set
            {
                point2 = value;
                TryUpdate();
            }
        }

        public override System.Windows.Shapes.Shape GetRawShape()
        {
            return shape;
        }

        public override SelectionBox CreateSelectionBox()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            shape.X1 = point1.X;
            shape.Y1 = -point1.Y;
            shape.X2 = point2.X;
            shape.Y2 = -point2.Y;
            base.Update();
        }

        public override bool IsPointInside(Point point)
        {
            System.Diagnostics.Debug.WriteLine("Try to test if a point is in a line.");
            return false;
        }

        public override void Move(Vector vector)
        {
            point1 += vector;
            point2 += vector;
            TryUpdate();
        }

        internal override void ShowProperty(ShapePropertyControl shapePropertyControl)
        {
            throw new Exception("ShapePropertyControl can't show property of a line.");
        }
    }

    public sealed class Circle : Shape
    {
        private Ellipse _shape = new Ellipse();
        private Point _center = new Point();
        private double _radius = 10.0;

        public Circle()
        {
            Update();
        }

        public override System.Windows.Shapes.Shape GetRawShape()
        {
            return _shape;
        }

        public override SelectionBox CreateSelectionBox()
        {
            return new CircleSelectionBox(this);
        }
        
        public override void Update()
        {
            _shape.Width = _radius * 2.0;
            _shape.Height = _radius * 2.0;
            Canvas.SetLeft(_shape, _center.X - _radius);
            Canvas.SetTop(_shape, -_center.Y - _radius);
            base.Update();
        }

        internal override void ShowProperty(ShapePropertyControl shapePropertyControl)
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
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException
                        ("Radius", value, "Radius can't be smaller than 0.");

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

    public sealed class Rectangle : Shape
    {
        private System.Windows.Shapes.Rectangle _shape = new System.Windows.Shapes.Rectangle();
        private double _left = -50.0;
        private double _top = 50.0;
        private double _right = 50.0;
        private double _bottom = -50.0;

        public Rectangle()
        {
            Update();
        }

        public override SelectionBox CreateSelectionBox()
        {
            return new RectangleSelectionBox(this);
        }

        public override void Update()
        {
            _shape.Width = _right - _left;
            _shape.Height = _top - _bottom;
            Canvas.SetLeft(_shape, _left);
            Canvas.SetTop(_shape, -_top);
            base.Update();
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

        public override System.Windows.Shapes.Shape GetRawShape()
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
                _left = value.X - halfWidth;
                _top = value.Y + halfHeight;
                _right = value.X + halfWidth;
                _bottom = value.Y - halfHeight;
                TryUpdate();
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
                if (value < Bottom)
                    throw new ArgumentOutOfRangeException
                        ("Top", value, "Top can't be smaller than Bottom.");
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
                if (value < Left)
                    throw new ArgumentOutOfRangeException
                        ("Right", value, "Right can't be smaller than Left.");
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
                if (value > Top)
                    throw new ArgumentOutOfRangeException
                        ("Bottom", value, "Bottom can't be bigger than Top.");
                _bottom = value;
                TryUpdate();
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

        public override void Move(Vector vector)
        {
            _left += vector.X;
            _right += vector.X;
            _top += vector.Y;
            _bottom += vector.Y;

            TryUpdate();
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
