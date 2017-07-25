using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CruPhysics.Shapes
{
    public abstract class SelectionBox : NotifyPropertyChangedObject
    {
        public class ControllerDraggedEventArgs : EventArgs
        {
            public ControllerDraggedEventArgs(Point position)
            {
                Position = position;
            }

            public Point Position { get; }
        }


        public class Controller : NotifyPropertyChangedObject
        {
            private const double radius = 4.0;


            private Cursor cursor;
            private ContextMenu contextMenu;

            public Controller()
            {
                Cursor = Cursors.SizeAll;
            }

            public Controller(Cursor cursor)
            {
                Cursor = cursor;
            }

            public BindablePoint Position { get; } = new BindablePoint();

            public double Radius => radius;

            public Cursor Cursor
            {
                get => cursor;
                set
                {
                    cursor = value;
                    RaisePropertyChangedEvent(nameof(Cursor));
                }
            }

            public ContextMenu ContextMenu
            {
                get => contextMenu;
                set
                {
                    contextMenu = value;
                    RaisePropertyChangedEvent(nameof(ContextMenu));
                }
            }

            public delegate void ControllerDraggedHandler(object sender, ControllerDraggedEventArgs e);

            private ControllerDraggedHandler dragged;

            public event ControllerDraggedHandler Dragged
            {
                add => dragged += value;
                remove => dragged -= value;
            }
        }



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
