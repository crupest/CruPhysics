using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace CruPhysics
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MainWindow window;
        private Scene scene;

        public MainViewModel(MainWindow window)
        {
            this.window = window;
            scene = new Scene(window);
        }

        public MainWindow Window
        {
            get
            {
                return window;
            }
        }

        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
        }

        private void RaisePropertyChangedEvent(string propertyName)
        {
            propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Scene Scene
        {
            get
            {
                return scene;
            }
        }
    }
}
