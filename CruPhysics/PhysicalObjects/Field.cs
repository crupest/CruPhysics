using CruPhysics.Shapes;
using System;
using System.Windows;

namespace CruPhysics.PhysicalObjects
{
    public abstract class Field : PhysicalObject
    {
        private CruShape shape = new CruRectangle();

        protected Field()
        {

        }

        public CruShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Shape));
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
            foreach (MovingObject movingObject in scene.ClassifiedObjects[typeof(MovingObject).Name])
            {
                Influence(movingObject, time);
            }
        }

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
        }
    }
}
