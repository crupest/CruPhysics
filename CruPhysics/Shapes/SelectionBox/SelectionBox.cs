using System.Collections.Generic;
using System.Windows.Controls;

namespace CruPhysics.Shapes.SelectionBox
{
    public abstract class SelectionBox : NotifyPropertyChangedObject
    {
        private ContextMenu contextMenu;

        public abstract IEnumerable<Controller> Controllers { get; }

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
