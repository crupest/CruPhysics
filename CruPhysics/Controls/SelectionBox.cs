using CruPhysics.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace CruPhysics.Controls
{
    public abstract class SelectionBox
    {

        public class ControllerDraggedEventArgs : EventArgs
        {
            public ControllerDraggedEventArgs(Point position)
            {
                Position = position;
            }

            public Point Position { get; }
        }

        public class Controller
        {
            private const double radius = 4.0;

            private CruCircle shape;

            public Controller()
            {
                shape = new CruCircle()
                {
                    Radius = radius
                };
            }

            public Controller(Cursor cursor)
                : this()
            {
                Cursor = cursor;
            }

            public Point Position => shape.Center;

            public Cursor Cursor { get; set; }

            public ContextMenu ContextMenu { get; set; }

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
                if (mouseRightButtonUp && ContextMenu != null)
                {
                    ContextMenu.PlacementTarget = shape.Raw;
                    ContextMenu.Placement = PlacementMode.MousePoint;
                    ContextMenu.IsOpen = true;
                }
                mouseRightButtonUp = false;
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
        private ContextMenu contextMenu;

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

        public abstract IEnumerable<Controller> Controllers { get; }

        public ContextMenu ContextMenu
        {
            get
            {
                return contextMenu;
            }
            set
            {
                contextMenu = value;
                foreach (var controller in Controllers)
                    controller.ContextMenu = value;
            }
        }

        public abstract void Delete();
    }


    public sealed class CircleSelectionBox : SelectionBox
    {
        private Controller centerController;
        private Controller radiusController;
        private Controller[] controllers = new Controller[2];

        private double radiusControllerAngle;

        private EventHandler updateEventHandler;

        public CircleSelectionBox(CruCircle circle)
            : base(circle)
        {
            centerController = new Controller(circle.Canvas, Cursors.SizeAll);
            radiusController = new Controller(circle.Canvas, Cursors.SizeAll);

            controllers[0] = centerController;
            controllers[1] = radiusController;

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

        public override IEnumerable<Controller> Controllers => controllers;

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

        public override IEnumerable<Controller> Controllers => controllers;

        public override void Delete()
        {
            foreach (var i in controllers)
                i.Delete();
            SelectedShape.Updated -= updateEventHandler;
        }
    }
}
