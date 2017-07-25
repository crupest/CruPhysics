using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using CruPhysics.Shapes;

namespace CruPhysics.Controls
{
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
