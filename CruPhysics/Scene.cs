using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CruPhysics
{
    public class Scene : NotifyPropertyChangedObject
    {
        private ObservableCollection<PhysicalObject> physicalObjects = new ObservableCollection<PhysicalObject>();
        private Dictionary<string, ObservableCollection<PhysicalObject>> classifiedObjects = new Dictionary<string, ObservableCollection<PhysicalObject>>();

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun = false;

        private PhysicalObject selectedObject = null;


        public Scene()
        {
            var list = PhysicalObjectManager.GetOrderedByRunRank();
            foreach (var k in list)
            {
                classifiedObjects.Add(k, new ObservableCollection<PhysicalObject>());
            }

            ScanInterval = TimeSpan.FromMilliseconds(15.0);
            timer.Tick += Run;
        }

        public PhysicalObject SelectedObject
        {
            get
            {
                return selectedObject;
            }
            set
            {

            }
        }

        public ObservableCollection<PhysicalObject> PhysicalObjects
        {
            get
            {
                return physicalObjects;
            }
        }

        public IDictionary<string, ObservableCollection<PhysicalObject>> ClassifiedObjects
        {
            get
            {
                return classifiedObjects;
            }
        }

        public void Add(PhysicalObject physicalObject)
        {
            physicalObject.AddToScene(this);
        }

        public void Remove(PhysicalObject physicalObject)
        {
            physicalObject.RemoveFromScene(this);
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
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => ScanInterval));
            }
        }

        public bool IsRunning
        {
            get
            {
                return timer.IsEnabled;
            }
            private set
            {
                timer.IsEnabled = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => IsRunning));
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
            private set
            {
                hasBegun = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => HasBegun));
            }
        }

        public TimeSpan RunningTime
        {
            get
            {
                return runningTime;
            }
            private set
            {
                runningTime = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => RunningTime));
            }
        }

        private void Run(object sender, EventArgs e)
        {
            var scanInterval = ScanInterval;

            RunningTime += scanInterval;

            foreach (var i in PhysicalObjects)
            {
                i.BeginOneScan(this);
            }

            foreach (var k in PhysicalObjectManager.GetOrderedByRunRank())
            {
                foreach (var o in ClassifiedObjects[k])
                    o.Run(this, scanInterval);
            }

            foreach (var i in PhysicalObjects)
            {
                i.FinishOneScan(this);
            }
        }

        public void Begin()
        {
            if (!HasBegun)
            {
                foreach (var i in physicalObjects)
                {
                    i.BeginRunning(this);
                }
                hasBegun = true;
            }
            IsRunning = true;
        }

        public void Stop()
        {
            foreach (var i in physicalObjects)
            {
                i.StopRunning(this);
            }
            IsRunning = false;
        }

        public void Restart()
        {
            if (IsRunning)
                Stop();
            RunningTime = TimeSpan.Zero;
            foreach (var i in physicalObjects)
            {
                i.Refresh(this);
            }
            HasBegun = false;
        }
    }
}
