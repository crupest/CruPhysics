using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace CruPhysics.Shapes
{
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
}
