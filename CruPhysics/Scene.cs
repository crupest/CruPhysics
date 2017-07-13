using CruPhysics.Controls;
using CruPhysics.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace CruPhysics
{
    public class Scene : NotifyPropertyChangedObject
    {
        private static Scene currentScene;

        public static Scene Current
        {
            get
            {
                return currentScene;
            }
            set
            {
                currentScene = value;
            }
        }

        private const string timeFormat = @"mm\:ss\.ff";


        private readonly MainWindow _mainWindow;

        internal ObservableCollection<PhysicalObject> physicalObjects = new ObservableCollection<PhysicalObject>();
        internal ObservableCollection<MovingObject> movingObjects = new ObservableCollection<MovingObject>();
        internal ObservableCollection<Field> fields = new ObservableCollection<Field>();

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun = false;


        private PhysicalObject selectedObject = null;
        private SelectionBox selectionBox = null;

        private void CreateSelectionBox()
        {
            selectionBox = selectedObject.Shape.CreateSelectionBox();
            selectionBox.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");
        }

        public PhysicalObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {
                if (selectedObject == value)
                    return;

                if (selectedObject != null)
                {
                    selectedObject.SetShowState(SelectionState.normal);
                    selectionBox.Delete();
                }
                selectedObject = value;
                if (value != null)
                {
                    value.SetShowState(SelectionState.select);
                    CreateSelectionBox();
                }

                RaisePropertyChangedEvent("SelectedObject");
            }
        }

        public Scene(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            Current = this;
            ScanInterval = TimeSpan.FromMilliseconds(15.0);
            timer.Tick += Run;
        }

        public ObservableCollection<PhysicalObject> PhysicalObjects
        {
            get
            {
                return physicalObjects;
            }
        }

        private void Run(object sender, EventArgs e)
        {
            runningTime += ScanInterval;
            RelatedMainWindow.TimeTextBox.Text = runningTime.ToString(timeFormat);

            var calculationInterval = ScanInterval;
            foreach (var i in movingObjects)
            {
                foreach (var j in fields)
                    j.Influence(i, calculationInterval);
                i.Run(calculationInterval);
                i.ClearForce();
            }

            foreach (var i in physicalObjects)
            {
                i.FinishCalculate(this);
            }
        }

        public MainWindow RelatedMainWindow
        {
            get
            {
                return _mainWindow;
            }
        }

        public Canvas RelatedWorldCanvas
        {
            get
            {
                return RelatedMainWindow.WorldCanvas;
            }
        }

        private void PhysicalObjectShapeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Shape" && ((PhysicalObject)sender).IsSelected)
            {
                selectionBox.Delete();
                CreateSelectionBox();
            }
        }

        public void Add(PhysicalObject physicalObject)
        {
            physicalObject.AddToScene(this);

            if (physicalObject is Field)
                physicalObject.PropertyChanged += PhysicalObjectShapeChanged;
        }

        public void Remove(PhysicalObject physicalObject)
        {
            physicalObject.RemoveFromScene(this);

            if (physicalObject is Field)
                physicalObject.PropertyChanged -= PhysicalObjectShapeChanged;
        }

        public double Bounds
        {
            get
            {
                return 1000.0;
            }
        }

        public TimeSpan ScanInterval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return timer.IsEnabled;
            }
        }

        /// <summary>
        /// <see cref="HasBegun"/> will be true even if it is paused.
        /// </summary>
        public bool HasBegun
        {
            get
            {
                return hasBegun;
            }
        }

        public TimeSpan RunningTime
        {
            get
            {
                return runningTime;
            }
        }

        public void Begin()
        {
            if (!hasBegun)
            {
                foreach (var i in physicalObjects)
                {
                    i.BeginRunning(this);
                }
                hasBegun = true;
            }
            RelatedMainWindow.TimeTextBox.Visibility = Visibility.Visible;
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
            foreach (var i in physicalObjects)
            {
                i.StopRunning(this);
            }
        }

        public void Restart()
        {
            if (IsRunning)
                Stop();
            hasBegun = false;
            runningTime = TimeSpan.Zero;
            foreach (var i in physicalObjects)
            {
                i.Refresh(this);
            }
            RelatedMainWindow.TimeTextBox.Visibility = Visibility.Collapsed;
            RelatedMainWindow.TimeTextBox.Text = runningTime.ToString(timeFormat);
        }
    }
}
