using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CruPhysics.Shapes.SelectionBox
{
    public sealed class CircleSelectionBox : SelectionBox
    {
        private readonly Controller centerController;
        private readonly Controller radiusController;

        private double radiusControllerAngle;

        public CircleSelectionBox(ICircle circle)
        {
            Shape = circle;
            centerController = new Controller(Cursors.SizeAll);
            radiusController = new Controller(Cursors.SizeAll);


            centerController.Dragged += CenterController_Dragged;
            radiusController.Dragged += RadiusController_Dragged;

            UpdateControllerPosition();

            Shape.PropertyChanged += ShapeOnPropertyChanged;
        }

        private void ShapeOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(ICircle.Center) ||
                args.PropertyName == nameof(ICircle.Radius))
                UpdateControllerPosition();
        }

        public override IShape SelectedShape => Shape;

        public ICircle Shape { get; }

        public override void UpdateControllerPosition()
        {
            centerController.Position.Set(Shape.Center);
            radiusController.Position.Set((Point)Shape.Center +
                                          Common.Rotate(new Vector(Shape.Radius, 0.0), radiusControllerAngle));
        }

        private void CenterController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            Shape.Center.Set(e.Position);
        }

        private void RadiusController_Dragged(object sender, ControllerDraggedEventArgs e)
        {
            var vector = e.Position - Shape.Center;
            Shape.Radius = vector.Length;
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

        protected override void DoDispose()
        {
            Shape.PropertyChanged -= ShapeOnPropertyChanged;
        }
    }
}
