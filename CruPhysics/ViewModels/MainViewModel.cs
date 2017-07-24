using CruPhysics.PhysicalObjects;

namespace CruPhysics.ViewModels
{
    public class MainViewModel : NotifyPropertyChangedObject
    {
        public MainViewModel()
        {
            Scene = new Scene();
        }

        public Scene Scene { get; }
    }
}
