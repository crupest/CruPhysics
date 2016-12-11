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
    public abstract class Shape
    {
        private bool _autoUpdate;

        public Shape()
        {
            _autoUpdate = false;
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

        public Canvas Canvas
        {
            get
            {
                return GetRawShape().Parent as Canvas;
            }

            set
            {
                if (Canvas == value)
                    return;

                if (Canvas != null)
                    Canvas.Children.Remove(GetRawShape());
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
            Canvas.Children.Remove(GetRawShape());
        }

        public System.Windows.Shapes.Shape Raw
        {
            get
            {
                return GetRawShape();
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

    public class Line : Shape
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

    public class Circle : Shape
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

    public class Rectangle : Shape
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
            throw new NotImplementedException();
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
