using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace CruPhysics
{
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        public NotifyPropertyChangedObject()
        {
            IsNotifying = true;
        }

        private PropertyChangedEventHandler propertyChanged;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged += value;
            }
            remove
            {
                propertyChanged -= value;
            }
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

        public static string GetPropertyName<TProperty>(Expression<Func<TProperty>> property)
        {
            var memberExpression = (MemberExpression)property.Body;
            return memberExpression.Member.Name;
        }

        public static object GetPropertyValue(object target, string propertyName)
        {
            return target.GetType().GetProperty(propertyName).GetValue(target);
        }

        public static void SetPropertyValue(object target, string propertyName, object value)
        {
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
            bool b = true;
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
