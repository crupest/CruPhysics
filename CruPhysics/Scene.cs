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
        private const string timeFormat = @"mm\:ss\.ff";


        internal ObservableCollection<PhysicalObject> physicalObjects = new ObservableCollection<PhysicalObject>();
        internal ObservableCollection<MovingObject> movingObjects = new ObservableCollection<MovingObject>();
        internal ObservableCollection<Field> fields = new ObservableCollection<Field>();

        private DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
        private TimeSpan runningTime = TimeSpan.Zero;
        private bool hasBegun = false;


        private PhysicalObject selectedObject = null;

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

        public Scene()
        {
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
            RunningTime += ScanInterval;

            var calculationInterval = ScanInterval;
            foreach (var i in movingObjects)
            {
                foreach (var j in fields)
                    j.Influence(i, calculationInterval);
                i.Run(calculationInterval);
            }

            foreach (var i in physicalObjects)
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
