using CruPhysics.Controls;
using System;
using System.Windows;

namespace CruPhysics.Shapes
{
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
                switch (args.PropertyName)
                {
                    case nameof(Left):
                        RaisePropertyChangedEvent(nameof(Right));
                        RaisePropertyChangedEvent(nameof(Lefttop));
                        RaisePropertyChangedEvent(nameof(Leftbottom));
                        RaisePropertyChangedEvent(nameof(Righttop));
                        RaisePropertyChangedEvent(nameof(Rightbottom));
                        RaisePropertyChangedEvent(nameof(Center));
                        return;
                    case nameof(Top):
                        RaisePropertyChangedEvent(nameof(Bottom));
                        RaisePropertyChangedEvent(nameof(Lefttop));
                        RaisePropertyChangedEvent(nameof(Leftbottom));
                        RaisePropertyChangedEvent(nameof(Righttop));
                        RaisePropertyChangedEvent(nameof(Rightbottom));
                        RaisePropertyChangedEvent(nameof(Center));
                        return;
                    case nameof(Width):
                        RaisePropertyChangedEvent(nameof(Right));
                        RaisePropertyChangedEvent(nameof(Righttop));
                        RaisePropertyChangedEvent(nameof(Rightbottom));
                        RaisePropertyChangedEvent(nameof(Center));
                        return;
                    case nameof(Height):
                        RaisePropertyChangedEvent(nameof(Bottom));
                        RaisePropertyChangedEvent(nameof(Leftbottom));
                        RaisePropertyChangedEvent(nameof(Rightbottom));
                        RaisePropertyChangedEvent(nameof(Center));
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
            get => width;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Width can't below 0.");
                width = value;
                RaisePropertyChangedEvent(nameof(Width));
            }
        }

        public double Height
        {
            get => height;
            set
            {
                if (value < 0.0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Height can't below 0.");
                height = value;
                RaisePropertyChangedEvent(nameof(Height));
            }
        }

        public double Left
        {
            get => left;
            set
            {
                left = value;
                RaisePropertyChangedEvent(nameof(Left));
            }
        }

        public double Top
        {
            get => top;
            set
            {
                top = value;
                RaisePropertyChangedEvent(nameof(Top));
            }
        }

        public double Right => Left + Width;

        public double Bottom => Top - Height;

        public Point Center => new Point(Left + Width / 2.0, Top - Height / 2.0);


        public Point Lefttop => new Point(Left, Top);

        public Point Righttop => new Point(Right, Top);

        public Point Leftbottom => new Point(Left, Bottom);

        public Point Rightbottom => new Point(Right, Bottom);

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
