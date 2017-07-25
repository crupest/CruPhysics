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

        public TimeSpan ScanInterval { get; }
    }

    public delegate void SceneScanEventHandler(object sender, SceneScanEventArgs args);

    public class Scene : NotifyPropertyChangedObject
    {
        private double bounds = 1000.0;

        private readonly Dictionary<string, ObservableCollection<PhysicalObject>> classifiedObjects = new Dictionary<string, ObservableCollection<PhysicalObject>>();

        private readonly DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun;

        private PhysicalObject selectedObject;


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
            get => selectedObject;
            set
            {
                selectedObject?.SetIsSelected(false);
                selectedObject = value;
                value?.SetIsSelected(true);
                RaisePropertyChangedEvent(nameof(SelectedObject));
            }
        }

        public ObservableCollection<PhysicalObject> PhysicalObjects { get; } = new ObservableCollection<PhysicalObject>();

        public IDictionary<string, ObservableCollection<PhysicalObject>> ClassifiedObjects => classifiedObjects;

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
            get => bounds;
            set
            {
                bounds = value;
                RaisePropertyChangedEvent(nameof(Bounds));
            }
        }

        public TimeSpan ScanInterval
        {
            get => timer.Interval;
            set
            {
                timer.Interval = value;
                RaisePropertyChangedEvent(nameof(ScanInterval));
            }
        }

        public bool IsRunning
        {
            get => timer.IsEnabled;
            private set
            {
                timer.IsEnabled = value;
                RaisePropertyChangedEvent(nameof(IsRunning));
            }
        }

        /// <summary>
        /// <see cref="HasBegun"/> will be true even if it is paused.
        /// </summary>
        public bool HasBegun
        {
            get => hasBegun;
            private set
            {
                hasBegun = value;
                RaisePropertyChangedEvent(nameof(HasBegun));
            }
        }

        public TimeSpan RunningTime
        {
            get => runningTime;
            private set
            {
                runningTime = value;
                RaisePropertyChangedEvent(nameof(RunningTime));
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
