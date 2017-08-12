using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CruPhysics.Shapes.SelectionBox
{
    public class ControllerDraggedEventArgs : EventArgs
    {
        public ControllerDraggedEventArgs(Point position)
        {
            Position = position;
        }

        public Point Position { get; }
    }

    public delegate void ControllerDraggedHandler(object sender, ControllerDraggedEventArgs e);

    public class Controller : NotifyPropertyChangedObject
    {
        private const double radius = 4.0;


        private Cursor cursor;

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

        internal void OnMove(Point newPosition)
        {
            dragged?.Invoke(this, new ControllerDraggedEventArgs(newPosition));
        }


        private ControllerDraggedHandler dragged;

        public event ControllerDraggedHandler Dragged
        {
            add => dragged += value;
            remove => dragged -= value;
        }
    }
}
