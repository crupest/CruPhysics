using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CruPhysics.Shapes.SelectionBox
{
    public sealed class RectangleSelectionBox : SelectionBox
    {
        //From left to right, from top to bottom.
        private readonly Controller[] controllers = new Controller[9];

        public RectangleSelectionBox(IRectangle rectangle)
        {
            Shape = rectangle;

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

            Shape.PropertyChanged += ShapeOnPropertyChanged;
        }

        private void ShapeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(IRectangle.Left) ||
                args.PropertyName == nameof(IRectangle.Top) ||
                args.PropertyName == nameof(IRectangle.Width) ||
                args.PropertyName == nameof(IRectangle.Height))
                UpdateControllerPosition();
        }

        public override IShape SelectedShape => Shape;

        public IRectangle Shape { get; }

        private void LefttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(Shape.GetRight(), e.Position.X);
            var top = Math.Max(Shape.GetBottom(), e.Position.Y);
            var width = Shape.GetRight() - left;
            var height = top - Shape.GetBottom();

            Shape.Left = left;
            Shape.Top = top;
            Shape.Width = width;
            Shape.Height = height;
        }

        private void TopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var top = Math.Max(Shape.GetBottom(), e.Position.Y);
            var height = top - Shape.GetBottom();

            Shape.Top = top;
            Shape.Height = height;
        }

        private void RighttopController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(Shape.Left, e.Position.X);
            var top = Math.Max(Shape.GetBottom(), e.Position.Y);
            var width = right - Shape.Left;
            var height = top - Shape.GetBottom();

            Shape.Top = top;
            Shape.Width = width;
            Shape.Height = height;
        }

        private void LeftController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(Shape.GetRight(), e.Position.X);
            var width = Shape.GetRight() - left;

            Shape.Left = left;
            Shape.Width = width;
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - Shape.GetCenter();

            Shape.Move(vector);
        }

        private void RightController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(Shape.Left, e.Position.X);
            var width = right - Shape.Left;

            Shape.Width = width;
        }

        private void LeftbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var left = Math.Min(Shape.GetRight(), e.Position.X);
            var bottom = Math.Min(Shape.Top, e.Position.Y);
            var width = Shape.GetRight() - left;
            var height = Shape.Top - bottom;

            Shape.Left = left;
            Shape.Width = width;
            Shape.Height = height;
        }

        private void BottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var bottom = Math.Min(Shape.Top, e.Position.Y);
            var height = Shape.Top - bottom;

            Shape.Height = height;
        }

        private void RightbottomController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var right = Math.Max(Shape.Left, e.Position.X);
            var bottom = Math.Min(Shape.Top, e.Position.Y);
            var width = right - Shape.Left;
            var height = Shape.Top - bottom;

            Shape.Width = width;
            Shape.Height = height;
        }

        public override void UpdateControllerPosition()
        {
            var rectangle = Shape;

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

        public override void Dispose()
        {
            Shape.PropertyChanged -= ShapeOnPropertyChanged;
        }
    }
}
