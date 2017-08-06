using System.Windows;
using CruPhysics.ViewModels;

namespace CruPhysics.Shapes
{
    public abstract class CruShape : ViewModelBase, IShape
    {
        public abstract void Move(Vector vector);
        public abstract bool IsPointInside(Point point);
    }
}
