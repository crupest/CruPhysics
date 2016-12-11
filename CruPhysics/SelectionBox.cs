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
using CruPhysics._SelectionBox;

namespace CruPhysics
{
    namespace _SelectionBox
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
                    ZIndex = 101
                };

                controller.MouseDown += OnMouseDown;
                controller.MouseUp   += OnMouseUp;
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
    }

    public abstract class SelectionBox
    {
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


    public class CircleSelectionBox : SelectionBox
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
            if (e.Position.X <= center.X)
                SelectedShape.Radius = 0.0;
            else
                SelectedShape.Radius = e.Position.X - center.Y;
            SelectedShape.Update();
        }

        public override void Delete()
        {
            base.Delete();
            centerController.Delete();
            radiusController.Delete();
        }
    }
}
