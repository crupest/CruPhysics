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

namespace CruPhysics
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
            private Circle controller;

            public Controller(Canvas canvas)
            {
                controller = new Circle()
                {
                    AutoUpdate = true,
                    Radius = 5.0,
                    Stroke = Brushes.Black,
                    Fill = Brushes.White,
                    Canvas = canvas,
                    ZIndex = PhysicalObjectZIndex.controller
                };

                controller.MouseDown += OnMouseDown;
                controller.MouseUp += OnMouseUp;
                controller.MouseMove += OnMouseMove;
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
                    return controller.Center;
                }

                set
                {
                    controller.Center = value;
                }
            }

            public Cursor Cursor
            {
                get
                {
                    return controller.Cursor;
                }

                set
                {
                    controller.Cursor = value;
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
                controller.Delete();
            }


            private static Vector cursorDelta;

            private void OnMouseDown(object sender, MouseButtonEventArgs args)
            {
                Mouse.Capture(controller.Raw);
                cursorDelta =
                    Common.TransformPoint(args.GetPosition(controller.Canvas)) - controller.Center;
                args.Handled = true;
            }

            private void OnMouseUp(object sender, MouseButtonEventArgs args)
            {
                Mouse.Capture(null);
                args.Handled = true;
            }

            private void OnMouseMove(object sender, MouseEventArgs args)
            {
                if (controller.Raw.IsMouseCaptured)
                {
                    var newCenter = Common.TransformPoint(args.GetPosition(controller.Canvas)) - cursorDelta;
                    controller.Center = newCenter;
                    dragged(this, new ControllerDraggedEventArgs(newCenter));
                }
            }
        }



        private Shape shape;

        public SelectionBox(Shape shape)
        {
            this.shape = shape;

            shape.Updated += UpdateView;
        }

        public Shape SelectedShape
        {
            get
            {
                return shape;
            }
        }

        protected abstract void UpdateView(object sender, EventArgs e);

        public virtual void Delete()
        {
            shape.Updated -= UpdateView;
        }
    }


    public sealed class CircleSelectionBox : SelectionBox
    {
        private Controller centerController;
        private Controller radiusController;

        public CircleSelectionBox(Circle circle)
            : base(circle)
        {
            centerController = new Controller(circle.Canvas, Cursors.SizeAll);
            radiusController = new Controller(circle.Canvas, Cursors.SizeWE);

            UpdateView(this, EventArgs.Empty);

            centerController.Dragged += CenterController_Dragged;
            radiusController.Dragged += RadiusController_Dragged;
        }

        public new Circle SelectedShape
        {
            get
            {
                return (Circle)base.SelectedShape;
            }
        }

        protected override void UpdateView(object sender, EventArgs e)
        {
            centerController.Position = SelectedShape.Center;
            radiusController.Position = SelectedShape.Center +
                new Vector(SelectedShape.Radius, 0.0);
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Center = e.Position;
            SelectedShape.Update();
        }

        private void RadiusController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var center = centerController.Position;
            SelectedShape.Radius =
                e.Position.X <= center.X ?
                0.0 : e.Position.X - center.Y;
            SelectedShape.Update();
        }

        public override void Delete()
        {
            base.Delete();
            centerController.Delete();
            radiusController.Delete();
        }
    }

    public sealed class RectangleSelectionBox : SelectionBox
    {
        Controller[] controllers = new Controller[9];
        //controllers[0] is lefttopController.
        //controllers[1] is topController.
        //controllers[2] is righttopController.
        //controllers[3] is leftController.
        //controllers[4] is centerController.
        //controllers[5] is rightController.
        //controllers[6] is leftbottomController.
        //controllers[7] is bottomController.
        //controllers[8] is rightbottomController.

        public RectangleSelectionBox(Rectangle rectangle)
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

            foreach (var i in controllers)
            {
                i.Dragged += UpdateAfterDraggedAndSetting;
            }

            UpdateView(this, EventArgs.Empty);
        }

        private void LefttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Left = Math.Min(SelectedShape.Right, e.Position.X);
            SelectedShape.Top = Math.Max(SelectedShape.Bottom, e.Position.Y);
        }

        private void TopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Top = Math.Max(SelectedShape.Bottom, e.Position.Y);
        }

        private void RighttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Right = Math.Max(SelectedShape.Left, e.Position.X);
            SelectedShape.Top = Math.Max(SelectedShape.Bottom, e.Position.Y);
        }

        private void LeftController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Left = Math.Min(SelectedShape.Right, e.Position.X);
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Center = e.Position;
        }

        private void RightController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Right = Math.Max(SelectedShape.Left, e.Position.X);
        }

        private void LeftbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Left = Math.Min(SelectedShape.Right, e.Position.X);
            SelectedShape.Bottom = Math.Min(SelectedShape.Top, e.Position.Y);
        }

        private void BottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Bottom = Math.Min(SelectedShape.Top, e.Position.Y);
        }

        private void RightbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Right = Math.Max(SelectedShape.Left, e.Position.X);
            SelectedShape.Bottom = Math.Min(SelectedShape.Top, e.Position.Y);
        }

        private void UpdateAfterDraggedAndSetting(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Update();
        }

        protected override void UpdateView(object sender, EventArgs e)
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

        public new Rectangle SelectedShape
        {
            get
            {
                return (Rectangle)base.SelectedShape;
            }
        }

        public override void Delete()
        {
            base.Delete();
            foreach (var i in controllers)
                i.Delete();
        }
    }
}
