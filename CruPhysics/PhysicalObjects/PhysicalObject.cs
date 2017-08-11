using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using CruPhysics.Shapes.SelectionBox;
using static CruPhysics.PhysicalObjects.PhysicalObjectUIResources;

namespace CruPhysics.PhysicalObjects
{
    public abstract class PhysicalObject : NotifyPropertyChangedObject
    {
        private Scene scene;
        private string name = string.Empty;
        private Color color = Common.GetRamdomColor();

        private bool isSelected;
        private bool isMouseHover;

        private Brush strokeBrush = NormalStrokeBrush;
        private double strokeThickness = NormalStrokeThickness;
        private int zIndex;

        protected PhysicalObject()
        {
            zIndex = this.GetMetadata().ZIndex;

            PropertyChanged += delegate(object sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName == nameof(IsSelected) || args.PropertyName == nameof(IsMouseHover))
                    UpdateStrokeView(IsSelected, IsMouseHover);
            };
        }

        private void UpdateStrokeView(bool isSelected, bool isMouseHover)
        {
            if (isSelected)
            {
                StrokeBrush = SelectStrokeBrush;
                StrokeThickness = SelectStrokeThickness;
                ZIndex = SelectedZIndex;
            }
            else
            {
                if (isMouseHover)
                {
                    StrokeBrush = HoverStrokeBrush;
                    StrokeThickness = HoverStrokeThickness;
                }
                else
                {
                    StrokeBrush = NormalStrokeBrush;
                    StrokeThickness = NormalStrokeThickness;
                }
                ZIndex = this.GetMetadata().ZIndex;
            }
        }

        public Scene RelatedScene
        {
            get => scene;
            private set
            {
                scene = value;
                RaisePropertyChangedEvent(nameof(RelatedScene));
            }
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChangedEvent(nameof(Color));
            }
        }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (IsSelected == value || RelatedScene == null)
                    return;

                RelatedScene.SelectedObject = value ? this : null;
            }
        }

        public bool IsMouseHover
        {
            get => isMouseHover;
            private set
            {
                isMouseHover = value;
                RaisePropertyChangedEvent(nameof(IsMouseHover));
            }
        }

        public Brush StrokeBrush
        {
            get => strokeBrush;
            private set
            {
                strokeBrush = value;
                RaisePropertyChangedEvent(nameof(StrokeBrush));
            }
        }

        public double StrokeThickness
        {
            get => strokeThickness;
            private set
            {
                strokeThickness = value;
                RaisePropertyChangedEvent(nameof(StrokeThickness));
            }
        }

        public int ZIndex
        {
            get => zIndex;
            private set
            {
                zIndex = value;
                RaisePropertyChangedEvent(nameof(ZIndex));
            }
        }

        public abstract void Run(Scene scene, TimeSpan time);
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();
        public abstract SelectionBox CreateSelectionBox();

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

        /// <summary>
        /// Only used in <see cref="Scene.set_SelectedObject"/>
        /// </summary>
        internal void SetIsSelected(bool value)
        {
            isSelected = value;
            RaisePropertyChangedEvent(nameof(IsSelected));
        }

        #region event handlers

        internal virtual void OnMouseEnter()
        {
            IsMouseHover = true;
        }

        internal virtual void OnMouseLeave()
        {
            IsMouseHover = false;
        }

        internal virtual void OnMouseDown()
        {
            IsSelected = true;
        }

        #endregion
    }
}
