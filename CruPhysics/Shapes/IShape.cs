using System.Windows;

namespace CruPhysics.Shapes
{
    public interface IShape : INotifyPropertyChangedEx
    {
        void Move(Vector vector);
    }
}
