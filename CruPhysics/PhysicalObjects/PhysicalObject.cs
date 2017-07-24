﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static CruPhysics.PhysicalObjects.PhysicalObjectUIResources;

namespace CruPhysics.PhysicalObjects
{
    public abstract class PhysicalObject : NotifyPropertyChangedObject
    {
        private Scene scene = null;
        private string name = string.Empty;
        private SelectionState selectionState = SelectionState.Normal;
        private Color color;

        public PhysicalObject()
        {
            Color = Common.GetRamdomColor();
        }

        public Scene RelatedScene
        {
            get
            {
                return scene;
            }
            private set
            {
                scene = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => RelatedScene));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Name));
            }
        }

        public SelectionState SelectionState
        {
            get
            {
                return selectionState;
            }
            set
            {
                selectionState = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => SelectionState));
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Color));
            }
        }

        public abstract void Run(Scene scene, TimeSpan time);
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();

        protected void PrepareShape(Shape shape)
        {
            shape.Cursor = Cursors.Arrow;
            shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");

            SetShowState(shape, SelectionState);
        }

        private void SetShowState(Shape shape, SelectionState selectionState)
        {
            shape.Stroke = StrokeBrushes[SelectionState];
            shape.StrokeThickness = StrokeThickness[SelectionState];
            if (selectionState == SelectionState.Select)
                Panel.SetZIndex(shape, SelectedZIndex);
            else
                Panel.SetZIndex(shape, this.GetMetadata().ZIndex);
        }

        internal void AddToScene(Scene scene)
        {
            OnAddToScene(scene);
        }

        internal void RemoveFromScene(Scene scene)
        {
            OnRemoveFromScene(scene);
        }

        protected virtual void OnAddToScene(Scene scene)
        {
            RelatedScene = scene;
            scene.PhysicalObjects.Add(this);
            scene.ClassifiedObjects[GetType().Name].Add(this);
        }

        protected virtual void OnRemoveFromScene(Scene scene)
        {
            RelatedScene = null;
            scene.PhysicalObjects.Remove(this);
            scene.ClassifiedObjects[GetType().Name].Remove(this);
        }
    }
}
