using CruPhysics.PhysicalObjects;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CruPhysics.ViewModels;

namespace CruPhysics.Windows
{
    public partial class MainWindow : Window
    {
        public static RoutedUICommand NewMovingObject   = new RoutedUICommand("运动对象(_0)",  "moving_object",    typeof(MainWindow));
        public static RoutedUICommand NewElectricField  = new RoutedUICommand("电场(_E)",     "electric_field",   typeof(MainWindow));
        public static RoutedUICommand NewMagneticField  = new RoutedUICommand("磁场(_M)",     "magnetic_field",   typeof(MainWindow));
        public static RoutedUICommand Property          = new RoutedUICommand("属性(_P)",     "property",         typeof(MainWindow));
        public static RoutedUICommand Delete            = new RoutedUICommand("删除(_D)",     "delete",           typeof(MainWindow));
        public static RoutedUICommand Begin             = new RoutedUICommand("开始(_B)",     "begin",            typeof(MainWindow));
        public static RoutedUICommand Stop              = new RoutedUICommand("停止(_S)",     "stop",             typeof(MainWindow));
        public static RoutedUICommand Restart           = new RoutedUICommand("重新(_R)",     "restart",          typeof(MainWindow));
        public static RoutedUICommand ResetView         = new RoutedUICommand("重置视图(_R)",  "reset_view",       typeof(MainWindow));


        public MainWindow()
        {
            InitializeComponent();

            ViewModel = (MainViewModel)FindResource("ViewModel");

            ObjectList.Focus();
        }

        public MainViewModel ViewModel { get; }

        private void NewMovingObject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Add(new MovingObject());
        }

        private void NewElectricField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Add(new ElectricField());
        }

        private void NewMagneticField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Add(new MagneticField());
        }

        private void Property_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = ViewModel.Scene.SelectedObject.CreatePropertyWindow();
            window.Owner = this;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Remove(ViewModel.Scene.SelectedObject);
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Scene.SelectedObject != null;
        }

        private void Begin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Begin();
        }

        private void Begin_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !ViewModel.Scene.IsRunning;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Stop();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Scene.IsRunning;
        }

        private void Restart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.Scene.Restart();
        }

        private void Restart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Scene.HasBegun; 
        }

        private void ListViewItem_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ListViewItem)sender).IsSelected = true;
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            var e = Common.FindAncestor(
                (DependencyObject)args.OriginalSource,
                //Next line makes me crazy.
                element => element is UIElement && ((UIElement)element).Focusable && !FocusManager.GetIsFocusScope(element) && !(element is MenuItem),
                true) as UIElement;
            if (e == null)
                ObjectList.Focus();
            else
                e.Focus();
        }

        private void ResetView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WorldCanvas.RenderTransform = Transform.Identity;
        }
    }


    public class CoordinateSystem
    {
        public CoordinateSystem(MainWindow window)
        {
            const double graduation = 50.0;

            var bounds = window.ViewModel.Scene.Bounds;
            var geometry = new GeometryGroup();

            for (var i = -graduation; i > -bounds; i -= graduation)
            {
                geometry.Children.Add(new LineGeometry(new Point(i, bounds), new Point(i, -bounds)));
                geometry.Children.Add(new LineGeometry(new Point(-bounds, i), new Point(bounds, i)));

                axisXScale.Add(CreateGraduationX(i));
                axisYScale.Add(CreateGraduationY(i));
            }

            for (var i = graduation; i < bounds; i += graduation)
            {
                geometry.Children.Add(new LineGeometry(new Point(i, bounds), new Point(i, -bounds)));
                geometry.Children.Add(new LineGeometry(new Point(-bounds, i), new Point(bounds, i)));

                axisXScale.Add(CreateGraduationX(i));
                axisYScale.Add(CreateGraduationY(i));
            }

            path.Data = geometry;
            path.Stroke = Brushes.Gray;

            zeroScale = CreateGraduation(0.0, new Point());

            axisX.X1 = -bounds;
            axisX.Y1 = 0;
            axisX.X2 = bounds;
            axisX.Y2 = 0;

            axisY.X1 = 0;
            axisY.Y1 = -bounds;
            axisY.X2 = 0;
            axisY.Y2 = bounds;

            axisX.Stroke = Brushes.Black;
            axisY.Stroke = Brushes.Black;
            axisX.StrokeThickness = 2.0;
            axisY.StrokeThickness = 2.0;

            window.WorldCanvas.Children.Add(path);
            window.WorldCanvas.Children.Add(axisX);
            window.WorldCanvas.Children.Add(axisY);

            window.WorldCanvas.Children.Add(zeroScale);

            foreach (var i in axisXScale)
                window.WorldCanvas.Children.Add(i);

            foreach (var i in axisYScale)
                window.WorldCanvas.Children.Add(i);
        }

        private static TextBlock CreateGraduation(double value, Point position)
        {
            var textBlock = new TextBlock()
            {
                Text = value.ToString(),
                Padding = new Thickness(2.0)
            };
            Canvas.SetLeft(textBlock, position.X);
            Canvas.SetTop(textBlock, position.Y);
            return textBlock;
        }

        private static TextBlock CreateGraduationX(double value)
        {
            return CreateGraduation(value, new Point(value, 0.0));
        }

        private static TextBlock CreateGraduationY(double value)
        {
            return CreateGraduation(value, new Point(0.0, -value));
        }

        private readonly Path path = new Path();
        private readonly Line axisX = new Line();
        private readonly Line axisY = new Line();
        private readonly TextBlock zeroScale;
        private readonly List<TextBlock> axisXScale = new List<TextBlock>();
        private readonly List<TextBlock> axisYScale = new List<TextBlock>();
    }

    public class ObjectListItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // ReSharper disable once PossibleNullReferenceException
            DataTemplate FindResource(string key) => (container as FrameworkElement).FindResource(key) as DataTemplate;

            if (item is MovingObject)
            {
                return FindResource("MovingObjectDataTemplate");
            }
            if (item is ElectricField)
            {
                return FindResource("ElectricFieldDataTemplate");
            }
            if (item is MagneticField)
            {
                return FindResource("MagneticFieldDataTemplate");
            }

            return base.SelectTemplate(item, container);
        }
    }
}
