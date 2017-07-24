using System.Windows;

namespace CruPhysics.PhysicalObjects
{
    public class Force : BindableVector
    {
        public Force()
        {

        }

        public Force(double x, double y)
            : base(x, y)
        {

        }

        public Force(Vector vector)
            : base(vector)
        {

        }
    }
}
