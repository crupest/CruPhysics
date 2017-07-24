using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace CruPhysics.PhysicalObjects
{
    public class SceneScanEventArgs : EventArgs
    {
        public SceneScanEventArgs(TimeSpan scanInterval)
        {
            ScanInterval = scanInterval;
        }

        public TimeSpan ScanInterval { get; private set; }
    }

    public delegate void SceneScanEventHandler(object sender, SceneScanEventArgs args);

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
            var eventArgs = new SceneScanEventArgs(scanInterval);

            RunningTime += scanInterval;

            BeforeOneScanEvent?.Invoke(this, eventArgs);

            foreach (var k in PhysicalObjectManager.GetOrderedByRunRank())
            {
                foreach (var o in ClassifiedObjects[k])
                    o.Run(this, scanInterval);
            }

            AfterOneScanEvent?.Invoke(this, eventArgs);
        }

        public void Begin()
        {
            if (!HasBegun)
            {
                BeginRunningEvent?.Invoke(this, EventArgs.Empty);
                hasBegun = true;
            }
            IsRunning = true;
        }

        public void Stop()
        {
            StopRunningEvent?.Invoke(this, EventArgs.Empty);
            IsRunning = false;
        }

        public void Restart()
        {
            if (IsRunning)
                Stop();
            RunningTime = TimeSpan.Zero;
            RefreshEvent?.Invoke(this, EventArgs.Empty);
            HasBegun = false;
        }

        public event SceneScanEventHandler BeforeOneScanEvent;
        public event SceneScanEventHandler AfterOneScanEvent;
        public event EventHandler BeginRunningEvent;
        public event EventHandler StopRunningEvent;
        public event EventHandler RefreshEvent;
    }
}
