using CruPhysics.Shapes;
using System;
using System.Windows;
using CruPhysics.Shapes.SelectionBox;

namespace CruPhysics.PhysicalObjects
{
    public abstract class Field : PhysicalObject
    {
        private CruShape shape;

        protected Field()
        {
            shape = new CruRectangle {Tag = this};
        }

        public CruShape Shape
        {
            get => shape;
            set
            {
                shape.Tag = null;
                shape = value;
                value.Tag = this;
                if (RelatedScene != null)
                {
                    RelatedScene.SelectionBox = CreateSelectionBox();
                }
                RaisePropertyChangedEvent(nameof(Shape));
            }
        }

        public override void Move(Vector vector)
        {
            Shape.Move(vector);
        }

        public bool IsMovingObjectInside(MovingObject movingObject)
        {
            return Shape.IsPointInside(movingObject.Position);
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (IsMovingObjectInside(movingObject))
                CalculateEffect(movingObject, time);
        }

        public abstract void CalculateEffect(MovingObject movingObject, TimeSpan time);

        public override void Run(Scene scene, TimeSpan time)
        {
            foreach (var o in scene.ClassifiedObjects[typeof(MovingObject).Name])
            {
                var movingObject = (MovingObject) o;
                Influence(movingObject, time);
            }
        }

        public override SelectionBox CreateSelectionBox()
        {
            if (Shape is IRectangle rectangle)
                return new RectangleSelectionBox(rectangle);
            if (Shape is ICircle circle)
                return new CircleSelectionBox(circle);
            return null;
        }
    }
}
