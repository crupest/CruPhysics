using System;
using System.Collections.Generic;

namespace CruPhysics.Shapes.SelectionBox
{
    public abstract class SelectionBox : NotifyPropertyChangedObject, IDisposable
    {
        private bool disposed;

        public abstract IShape SelectedShape { get; }
        public abstract IEnumerable<Controller> Controllers { get; }
        public abstract void UpdateControllerPosition();
        protected abstract void DoDispose();

        public void Dispose()
        {
            if (disposed)
                return;

            DoDispose();
            disposed = true;
        }
    }
}
