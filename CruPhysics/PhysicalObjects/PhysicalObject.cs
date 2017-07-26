using System;
using System.Windows;
using System.Windows.Media;

namespace CruPhysics.PhysicalObjects
{
    public abstract class PhysicalObject : NotifyPropertyChangedObject
    {
        private Scene scene;
        private string name = string.Empty;
        private Color color;

        private bool isSelected;
        private bool isMouseHover;

        protected PhysicalObject()
        {
            Color = Common.GetRamdomColor();
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

        public abstract void Run(Scene scene, TimeSpan time);
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();

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
    }
}
