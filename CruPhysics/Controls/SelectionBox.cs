using CruPhysics.Shapes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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


        public class Controller : NotifyPropertyChangedObject
        {
            private const double radius = 4.0;


            private Cursor cursor;
            private ContextMenu contextMenu;

            public Controller()
            {
                Cursor = Cursors.SizeAll;
            }

            public Controller(Cursor cursor)
            {
                Cursor = cursor;
            }

            public BindablePoint Position { get; } = new BindablePoint();

            public double Radius => radius;

            public Cursor Cursor
            {
                get => cursor;
                set
                {
                    cursor = value;
                    RaisePropertyChangedEvent(nameof(Cursor));
                }
            }

            public ContextMenu ContextMenu
            {
                get => contextMenu;
                set
                {
                    contextMenu = value;
                    RaisePropertyChangedEvent(nameof(ContextMenu));
                }
            }

            public delegate void ControllerDraggedHandler(object sender, ControllerDraggedEventArgs e);

            private ControllerDraggedHandler dragged;

            public event ControllerDraggedHandler Dragged
            {
                add => dragged += value;
                remove => dragged -= value;
            }
        }



        private ContextMenu contextMenu;

        public abstract IEnumerable<Controller> Controllers { get; }

        public ContextMenu ContextMenu
        {
            get => contextMenu;
            set
            {
                contextMenu = value;
                foreach (var controller in Controllers)
                    controller.ContextMenu = value;
            }
        }
    }


    public sealed class CircleSelectionBox : SelectionBox
    {
        private readonly Controller centerController;
        private readonly Controller radiusController;

        private double radiusControllerAngle;

        public CircleSelectionBox(CruCircle circle)
        {
            SelectedShape = circle;
            centerController = new Controller(Cursors.SizeAll);
            radiusController = new Controller(Cursors.SizeAll);

            UpdateControllerPosition();

            centerController.Dragged += CenterController_Dragged;
            radiusController.Dragged += RadiusController_Dragged;
        }

        public CruCircle SelectedShape { get; }

        private void UpdateControllerPosition()
        {
            centerController.Position.Set(SelectedShape.Center);
            radiusController.Position.Set((Point)SelectedShape.Center +
                Common.Rotate(new Vector(SelectedShape.Radius, 0.0), radiusControllerAngle));
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            SelectedShape.Center.Set(e.Position);
        }

        private void RadiusController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - SelectedShape.Center;
            SelectedShape.Radius = vector.Length;
            radiusControllerAngle = Common.GetAngleBetweenXAxis(vector);
        }

        public override IEnumerable<Controller> Controllers
        {
            get
            {
                yield return centerController;
                yield return radiusController;
            }
        }
    }

    public sealed class RectangleSelectionBox : SelectionBox
    {
        //From left to right, from top to bottom.
        private readonly Controller[] controllers = new Controller[9];

        public RectangleSelectionBox(IRectangle rectangle)
        {
            SelectedShape = rectangle;

            controllers[0] = new Controller(Cursors.SizeNWSE);
            controllers[1] = new Controller(Cursors.SizeNS);
            controllers[2] = new Controller(Cursors.SizeNESW);
            controllers[3] = new Controller(Cursors.SizeWE);
            controllers[4] = new Controller(Cursors.SizeAll);
            controllers[5] = new Controller(Cursors.SizeWE);
            controllers[6] = new Controller(Cursors.SizeNESW);
            controllers[7] = new Controller(Cursors.SizeNS);
            controllers[8] = new Controller(Cursors.SizeNWSE);

            controllers[0].Dragged += LefttopController_Dragged;
            controllers[1].Dragged += TopController_Dragged;
            controllers[2].Dragged += RighttopController_Dragged;
            controllers[3].Dragged += LeftController_Dragged;
            controllers[4].Dragged += CenterController_Dragged;
            controllers[5].Dragged += RightController_Dragged;
            controllers[6].Dragged += LeftbottomController_Dragged;
            controllers[7].Dragged += BottomController_Dragged;
            controllers[8].Dragged += RightbottomController_Dragged;

            UpdateControllerPosition();
        }

        public IRectangle SelectedShape { get; }

        private void LefttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(SelectedShape.GetRight(), e.Position.X);
            var top = Math.Max(SelectedShape.GetBottom(), e.Position.Y);
            var width = SelectedShape.GetRight() - left;
            var height = top - SelectedShape.GetBottom();

            SelectedShape.Left = left;
            SelectedShape.Top = top;
            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }

        private void TopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var top = Math.Max(SelectedShape.GetBottom(), e.Position.Y);
            var height = top - SelectedShape.GetBottom();

            SelectedShape.Top = top;
            SelectedShape.Height = height;
        }

        private void RighttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(SelectedShape.Left, e.Position.X);
            var top = Math.Max(SelectedShape.GetBottom(), e.Position.Y);
            var width = right - SelectedShape.Left;
            var height = top - SelectedShape.GetBottom();

            SelectedShape.Top = top;
            SelectedShape.Width = width;
            SelectedShape.Height = height;
        }

        private void LeftController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(SelectedShape.GetRight(), e.Position.X);
            var width = SelectedShape.GetRight() - left;

            SelectedShape.Left = left;
            SelectedShape.Width = width;
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - SelectedShape.GetCenter();

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
            var left = Math.Min(SelectedShape.GetRight(), e.Position.X);
            var bottom = Math.Min(SelectedShape.Top, e.Position.Y);
            var width = SelectedShape.GetRight() - left;
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

            controllers[0].Position.Set(rectangle.GetLefttop());
            controllers[1].Position.Set(new Point((rectangle.Left + rectangle.GetRight()) / 2.0, rectangle.Top));
            controllers[2].Position.Set(rectangle.GetRighttop());
            controllers[3].Position.Set(new Point(rectangle.Left, (rectangle.Top + rectangle.GetBottom()) / 2.0));
            controllers[4].Position.Set(rectangle.GetCenter());
            controllers[5].Position.Set(new Point(rectangle.GetRight(), (rectangle.Top + rectangle.GetBottom()) / 2.0));
            controllers[6].Position.Set(rectangle.GetLeftbottom());
            controllers[7].Position.Set(new Point((rectangle.Left + rectangle.GetRight()) / 2.0, rectangle.GetBottom()));
            controllers[8].Position.Set(rectangle.GetRightbottom());
        }

        public override IEnumerable<Controller> Controllers => controllers;
    }
}
