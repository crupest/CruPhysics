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
using CruPhysics.Controls;

namespace CruPhysics.Shapes
{
    public abstract class CruShape : NotifyPropertyChangedObject
    {
        protected CruShape()
        {

        }

        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);

        public abstract SelectionBox CreateSelectionBox();
    }

    public sealed class CruLine : CruShape
    {
        private BindablePoint point1 = new BindablePoint();
        private BindablePoint point2 = new BindablePoint();

        public CruLine()
        {

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

        public override SelectionBox CreateSelectionBox()
        {
            throw new NotImplementedException();
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
    }

    public sealed class CruCircle : CruShape
    {
        private BindablePoint center = new BindablePoint();
        private double radius = 10.0;

        public CruCircle()
        {

        }

        public override SelectionBox CreateSelectionBox()
        {
            return new CircleSelectionBox(this);
        }

        public BindablePoint Center
        {
            get
            {
                return center;
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
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException
                        ("Radius", value, "Radius can't be smaller than 0.");

                radius = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Radius));
            }
        }
        
        public override void Move(Vector vector)
        {
            Center.Move(vector);
        }

        public override bool IsPointInside(Point point)
        {
            return Math.Pow(point.X - Center.X, 2) +
                Math.Pow(point.Y - Center.Y, 2) <= Math.Pow(Radius, 2);
        }
    }

    public sealed class CruRectangle : CruShape
    {
        private double left = -50.0;
        private double top = 50.0;
        private double width = 100.0;
        private double height = 100.0;

        public CruRectangle()
        {
            PropertyChanged += (sender, args) => 
            {
                if (args.PropertyName == "Left")
                {
                    RaisePropertyChangedEvent("Right");
                    RaisePropertyChangedEvent("LeftTop");
                    RaisePropertyChangedEvent("LeftBottom");
                    RaisePropertyChangedEvent("RightTop");
                    RaisePropertyChangedEvent("RightBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Top")
                {
                    RaisePropertyChangedEvent("Bottom");
                    RaisePropertyChangedEvent("LeftTop");
                    RaisePropertyChangedEvent("LeftBottom");
                    RaisePropertyChangedEvent("RightTop");
                    RaisePropertyChangedEvent("RightBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Width")
                {
                    RaisePropertyChangedEvent("Right");
                    RaisePropertyChangedEvent("RightTop");
                    RaisePropertyChangedEvent("RightBottom");
                    RaisePropertyChangedEvent("Center");
                    return;
                }

                if (args.PropertyName == "Height")
                {
                    RaisePropertyChangedEvent("Bottom");
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

        public double Width
        {
            get
            {
                return width;
            }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("value", value, "Width can't below 0.");
                width = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Width));
            }
        }

        public double Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException("value", value, "Height can't below 0.");
                height = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Height));
            }
        }

        public double Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Left));
            }
        }

        public double Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Top));
            }
        }

        public double Right
        {
            get
            {
                return Left + Width;
            }
        }

        public double Bottom
        {
            get
            {
                return Top - Height;
            }
        }

        public Point Center
        {
            get
            {
                return new Point(Left + Width / 2.0, Top - Height / 2.0);
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
            Left += vector.X;
            Top += vector.Y;
        }

        public override bool IsPointInside(Point point)
        {
            return
                point.X > Left &&
                point.X < Right &&
                point.Y > Bottom &&
                point.Y < Top;
        }
    }
}
