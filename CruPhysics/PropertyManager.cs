using System.ComponentModel;

namespace CruPhysics
{
    public interface INotifyPropertyChangedEx : INotifyPropertyChanged
    {
        bool IsNotifying { get; set; }
    }

    public class NotifyPropertyChangedObject : INotifyPropertyChangedEx
    {
        public NotifyPropertyChangedObject()
        {
            IsNotifying = true;
        }

        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => propertyChanged += value;
            remove => propertyChanged -= value;
        }

        public bool IsNotifying { get; set; }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            if (IsNotifying)
                propertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public static class PropertyManager
    {
        public static object Object { get; set; }

        public static object GetPropertyValue(object target, string propertyName)
        {
            // ReSharper disable once PossibleNullReferenceException
            return target.GetType().GetProperty(propertyName).GetValue(target);
        }

        public static void SetPropertyValue(object target, string propertyName, object value)
        {
            // ReSharper disable once PossibleNullReferenceException
            target.GetType().GetProperty(propertyName).SetValue(target, value);
        }

        public static void OneWayBind(INotifyPropertyChanged source, string sourcePropertyName, object target, string targetPropertyName)
        {
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == sourcePropertyName)
                {
                    SetPropertyValue(target, targetPropertyName, GetPropertyValue(source, sourcePropertyName));
                }
            };
        }

        public static void TwoWayBind(INotifyPropertyChanged source, string sourcePropertyName, INotifyPropertyChanged target, string targetPropertyName)
        {
            var b = true;
            source.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == sourcePropertyName && b)
                {
                    b = !b;
                    SetPropertyValue(target, targetPropertyName, GetPropertyValue(source, sourcePropertyName));
                }
                else
                {
                    b = !b;
                }
            };
            target.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == targetPropertyName && b)
                {
                    b = !b;
                    SetPropertyValue(source, sourcePropertyName, GetPropertyValue(target, targetPropertyName));
                }
                else
                {
                    b = !b;
                }
            };
        }
    }
}
