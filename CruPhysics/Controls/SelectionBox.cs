using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using CruPhysics.Shapes;

namespace CruPhysics.Controls
{
    public abstract class SelectionBox
    {

        internal class ControllerDraggedEventArgs : EventArgs
        {
            private Point position;

            internal ControllerDraggedEventArgs(Point position)
                : base()
            {
                this.position = position;
            }

            public Point Position
            {
                get
                {
                    return position;
                }
            }
        }

        internal class Controller
        {
            private const double radius = 4.0;

            private CruCircle shape;

            public Controller(Canvas canvas)
            {
                shape = new CruCircle()
                {
                    Radius = radius,
                    Stroke = Brushes.Black,
                    Fill = Brushes.White,
                    Canvas = canvas,
                    ZIndex = PhysicalObjectZIndex.Controller
                };

                shape.MouseDown += OnMouseDown;
                shape.MouseUp += OnMouseUp;
                shape.MouseMove += OnMouseMove;
            }

            public Controller(Canvas canvas, Cursor cursor)
                : this(canvas)
            {
                Cursor = cursor;
            }

            public Point Position
            {
                get
                {
                    return (Point)shape.Center;
                }
                set
                {
                    shape.Center.Set(value);
                }
            }

            public Cursor Cursor
            {
                get
                {
                    return shape.Cursor;
                }

                set
                {
                    shape.Cursor = value;
                }
            }

            public delegate void ControllerDraggedHandler(object sender, ControllerDraggedEventArgs e);

            private ControllerDraggedHandler dragged;

            public event ControllerDraggedHandler Dragged
            {
                add
                {
                    dragged += value;
                }

                remove
                {
                    dragged -= value;
                }
            }

            public void Delete()
            {
                shape.Delete();
            }


            private static Vector cursorDelta;

            private void OnMouseDown(object sender, ShapeMouseEventArgs args)
            {
                Mouse.Capture(shape.Raw);
                cursorDelta = args.Position - Position;
                args.Raw.Handled = true;
            }

            private void OnMouseUp(object sender, ShapeMouseEventArgs args)
            {
                Mouse.Capture(null);
                args.Raw.Handled = true;
            }

            private void OnMouseMove(object sender, ShapeMouseEventArgs args)
            {
                if (shape.Raw.IsMouseCaptured)
                {
                    dragged(this, new ControllerDraggedEventArgs(args.Position - cursorDelta));
                }
            }
        }



        private CruShape selectedShape;

        public SelectionBox(CruShape shape)
        {
            selectedShape = shape;
        }

        public CruShape SelectedShape
        {
            get
            {
                return selectedShape;
            }
        }

        public abstract void Delete();
    }


    public sealed class CircleSelectionBox : SelectionBox
    {
        private Controller centerController;
        private Controller radiusController;

        private double radiusControllerAngle;

        private EventHandler updateEventHandler;

        public CircleSelectionBox(CruCircle circle)
            : base(circle)
        {
            centerController = new Controller(circle.Canvas, Cursors.SizeAll);
            radiusController = new Controller(circle.Canvas, Cursors.SizeAll);

            UpdateControllerPosition();

            updateEventHandler = (sender, args) => UpdateControllerPosition();

            centerController.Dragged += CenterController_Dragged;
            radiusController.Dragged += RadiusController_Dragged;

            circle.Updated += updateEventHandler;
        }

        public new CruCircle SelectedShape
        {
            get
            {
                return (CruCircle)base.SelectedShape;
            }
        }

        private void UpdateControllerPosition()
        {
            centerController.Position = (Point) SelectedShape.Center;
            radiusController.Position = (Point) SelectedShape.Center +
                Common.Rotate(new Vector(SelectedShape.Radius, 0.0), radiusControllerAngle);
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Center.Set(e.Position);
        }

        private void RadiusController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - (Point)SelectedShape.Center;
            SelectedShape.Radius = vector.Length;
            radiusControllerAngle = Common.GetAngleBetweenXAxis(vector);
        }

        public override void Delete()
        {
            centerController.Delete();
            radiusController.Delete();
            SelectedShape.Updated -= updateEventHandler;
        }
    }

    public sealed class RectangleSelectionBox : SelectionBox
    {
        //From left to right, from top to bottom.
        private Controller[] controllers = new Controller[9];

        private EventHandler updateEventHandler;

        public RectangleSelectionBox(CruRectangle rectangle)
            : base(rectangle)
        {
            var canvas = rectangle.Canvas;

            controllers[0] = new Controller(canvas, Cursors.SizeNWSE);
            controllers[1] = new Controller(canvas, Cursors.SizeNS);
            controllers[2] = new Controller(canvas, Cursors.SizeNESW);
            controllers[3] = new Controller(canvas, Cursors.SizeWE);
            controllers[4] = new Controller(canvas, Cursors.SizeAll);
            controllers[5] = new Controller(canvas, Cursors.SizeWE);
            controllers[6] = new Controller(canvas, Cursors.SizeNESW);
            controllers[7] = new Controller(canvas, Cursors.SizeNS);
            controllers[8] = new Controller(canvas, Cursors.SizeNWSE);

            controllers[0].Dragged += LefttopController_Dragged;
            controllers[1].Dragged += TopController_Dragged;
            controllers[2].Dragged += RighttopController_Dragged;
            controllers[3].Dragged += LeftController_Dragged;
            controllers[4].Dragged += CenterController_Dragged;
            controllers[5].Dragged += RightController_Dragged;
            controllers[6].Dragged += LeftbottomController_Dragged;
            controllers[7].Dragged += BottomController_Dragged;
            controllers[8].Dragged += RightbottomController_Dragged;

            updateEventHandler = (sender, args) => UpdateControllerPosition();
            rectangle.Updated += updateEventHandler;

            UpdateControllerPosition();
        }

        private void LefttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(SelectedShape.Right, e.Position.X);
            var top = Math.Max(SelectedShape.Bottom, e.Position.Y);
            var width = SelectedShape.Right - left;
            var height = top - SelectedShape.Bottom;

            SelectedShape.Left = left;
            SelectedShape.Top = top;
            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }

        private void TopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var top = Math.Max(SelectedShape.Bottom, e.Position.Y);
            var height = top - SelectedShape.Bottom;

            SelectedShape.Top = top;
            SelectedShape.Height = height;
        }

        private void RighttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(SelectedShape.Left, e.Position.X);
            var top = Math.Max(SelectedShape.Bottom, e.Position.Y);
            var width = right - SelectedShape.Left;
            var height = top - SelectedShape.Bottom;

            SelectedShape.Top = top;
            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }

        private void LeftController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(SelectedShape.Right, e.Position.X);
            var width = SelectedShape.Right - left;

            SelectedShape.Left = left;
            SelectedShape.Width = width;
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - SelectedShape.Center;

            SelectedShape.Move(vector);
        }

        private void RightController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(SelectedShape.Left, e.Position.X);
            var width = right - SelectedShape.Left;

            SelectedShape.Width = width;
        }

        private void LeftbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(SelectedShape.Right, e.Position.X);
            var bottom = Math.Min(SelectedShape.Top, e.Position.Y);
            var width = SelectedShape.Right - left;
            var height = SelectedShape.Top - bottom;

            SelectedShape.Left = left;
            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }

        private void BottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var bottom = Math.Min(SelectedShape.Top, e.Position.Y);
            var height = SelectedShape.Top - bottom;

            SelectedShape.Height = height;
        }

        private void RightbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(SelectedShape.Left, e.Position.X);
            var bottom = Math.Min(SelectedShape.Top, e.Position.Y);
            var width = right - SelectedShape.Left;
            var height = SelectedShape.Top - bottom;

            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }
        
        private void UpdateControllerPosition()
        {
            var rectangle = SelectedShape;

            controllers[0].Position = rectangle.Lefttop;
            controllers[1].Position = new Point((rectangle.Left + rectangle.Right) / 2.0, rectangle.Top);
            controllers[2].Position = rectangle.Righttop;
            controllers[3].Position = new Point(rectangle.Left, (rectangle.Top + rectangle.Bottom) / 2.0);
            controllers[4].Position = rectangle.Center;
            controllers[5].Position = new Point(rectangle.Right, (rectangle.Top + rectangle.Bottom) / 2.0);
            controllers[6].Position = rectangle.Leftbottom;
            controllers[7].Position = new Point((rectangle.Left + rectangle.Right) / 2.0, rectangle.Bottom);
            controllers[8].Position = rectangle.Rightbottom;
        }

        public new CruRectangle SelectedShape
        {
            get
            {
                return (CruRectangle)base.SelectedShape;
            }
        }

        public override void Delete()
        {
            foreach (var i in controllers)
                i.Delete();
            SelectedShape.Updated -= updateEventHandler;
        }
    }
}
