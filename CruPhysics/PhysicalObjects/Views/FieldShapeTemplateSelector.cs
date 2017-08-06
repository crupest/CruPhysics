using System.Windows;
using System.Windows.Controls;
using CruPhysics.Shapes;

namespace CruPhysics.PhysicalObjects.Views
{
    public class FieldShapeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate GetDataTemplate(string key)
            {
                return ((FrameworkElement) container).FindResource(key) as DataTemplate;
            }

            if (item is CruRectangle)
            {
                return GetDataTemplate("FieldCruRectangleTemplate");
            }

            if (item is CruShape)
            {
                return GetDataTemplate("FieldCruCircleTemplate");
            }
            
            return base.SelectTemplate(item, container);
        }
    }
}
