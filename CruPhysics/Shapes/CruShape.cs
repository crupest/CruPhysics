using CruPhysics.Controls;
using System.Windows;

namespace CruPhysics.Shapes
{
    public abstract class CruShape : NotifyPropertyChangedObject
    {
        protected CruShape()
        {

        }

        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);

        public abstract SelectionBox CreateSelectionBox();
    }
}
