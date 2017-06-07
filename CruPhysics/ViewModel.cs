using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using CruPhysics.Windows;

namespace CruPhysics
{
    public class MainViewModel : NotifyPropertyChangedObject
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

        public Scene Scene
        {
            get
            {
                return scene;
            }
        }
    }
}
