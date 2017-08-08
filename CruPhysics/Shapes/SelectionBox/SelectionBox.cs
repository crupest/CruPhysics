using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CruPhysics.Shapes.SelectionBox
{
    public abstract class SelectionBox : NotifyPropertyChangedObject, IDisposable
    {
        private ContextMenu contextMenu;

        public abstract IShape SelectedShape { get; }
        public abstract IEnumerable<Controller> Controllers { get; }
        public abstract void UpdateControllerPosition();
        public abstract void Dispose();

        public ContextMenu ContextMenu
        {
            get => contextMenu;
            set
            {
                contextMenu = value;
                foreach (var controller in Controllers)
                    controller.ContextMenu = value;
            }
        }
    }
}
