using System.Windows;

namespace CruPhysics.Shapes
{
    public abstract class CruShape : NotifyPropertyChangedObject, IShape
    {
        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);
    }
}
