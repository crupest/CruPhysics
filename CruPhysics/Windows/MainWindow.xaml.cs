using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CruPhysics.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
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

        private MainViewModel viewModel;
        private CoordinateSystem coordinateSystem;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel(this);

            coordinateSystem = new CoordinateSystem(this);

            ObjectList.Focus();
        }

        public MainViewModel ViewModel
        {
            get
            {
                return viewModel;
            }
        }

        private void NewMovingObject_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            MovingObject moving_object = new MovingObject();
            viewModel.Scene.Add(moving_object);
        }

        private void NewElectricField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var electric_field = new ElectricField();
            viewModel.Scene.Add(electric_field);
        }

        private void NewMagneticField_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var magnetic_field = new MagneticField();
            viewModel.Scene.Add(magnetic_field);
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
            viewModel.Scene.Remove(ViewModel.Scene.SelectedObject);
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.Scene.SelectedObject != null;
        }

        private void Begin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.Scene.Begin();
        }

        private void Begin_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !viewModel.Scene.IsRunning;
        }

        private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.Scene.Stop();
        }

        private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.Scene.IsRunning;
        }

        private void Restart_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.Scene.Restart();
        }

        private void Restart_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = viewModel.Scene.HasBegun; 
        }

        private void MainCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var matrix = WorldCanvas.RenderTransform.Value;
            matrix.Translate(-(e.PreviousSize.Width / 2.0), -(e.PreviousSize.Height / 2.0));
            matrix.Translate(e.NewSize.Width / 2.0, e.NewSize.Height / 2.0);
            WorldCanvas.RenderTransform = new MatrixTransform(matrix);
        }

        private Point mainCanvasMousePosition;

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.Scene.SelectedObject = null;

            mainCanvasMousePosition = e.GetPosition(MainCanvas);
            MainCanvas.CaptureMouse();
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainCanvas.IsMouseCaptured)
            {
                Func<Point, Point> TransformPoint = (point) => WorldCanvas.RenderTransform.Transform(point);
                var displacement = TransformPoint(e.GetPosition(MainCanvas)) - TransformPoint(mainCanvasMousePosition);
                var matrix = WorldCanvas.RenderTransform.Value;
                matrix.Translate(displacement.X, displacement.Y);
                WorldCanvas.RenderTransform = new MatrixTransform(matrix);
                mainCanvasMousePosition = e.GetPosition(MainCanvas);
            }
        }

        private void MainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainCanvas.ReleaseMouseCapture();
        }

        private void ObjectList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var hitTestResult = VisualTreeHelper.HitTest(ObjectList, e.GetPosition(ObjectList));
            var listViewItem  = Common.FindAncestor(hitTestResult.VisualHit, (element) => element is ListViewItem);
            if (listViewItem == null)
            {
                ObjectList.UnselectAll();
                ObjectList.Focus();
            }
        }

        private void ListViewItem_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ListViewItem)sender).IsSelected = true;
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            var e = Common.FindAncestor((DependencyObject)args.OriginalSource, element => element is UIElement && ((UIElement)element).Focusable, true) as UIElement;
            if (e == null)
                ObjectList.Focus();
            else
                e.Focus();
        }

        private void ResetView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WorldCanvas.RenderTransform = new TranslateTransform(MainCanvas.ActualWidth / 2.0, MainCanvas.ActualHeight / 2.0);
        }
    }


    public class CoordinateSystem
    {
        public CoordinateSystem(MainWindow window)
        {
            var bounds = window.ViewModel.Scene.Bounds;
            var graduation = 50.0;
            var geometry = new GeometryGroup();

            for (var i = -graduation; i > -bounds; i -= graduation)
            {
                geometry.Children.Add(new LineGeometry(new Point(i, bounds), new Point(i, -bounds)));
                geometry.Children.Add(new LineGeometry(new Point(-bounds, i), new Point(bounds, i)));

                _axisXScale.Add(CreateGraduationX(i));
                _axisYScale.Add(CreateGraduationY(i));
            }

            for (var i = graduation; i < bounds; i += graduation)
            {
                geometry.Children.Add(new LineGeometry(new Point(i, bounds), new Point(i, -bounds)));
                geometry.Children.Add(new LineGeometry(new Point(-bounds, i), new Point(bounds, i)));

                _axisXScale.Add(CreateGraduationX(i));
                _axisYScale.Add(CreateGraduationY(i));
            }

            _path.Data = geometry;
            _path.Stroke = Brushes.Gray;

            _zeroScale = CreateGraduation(0.0, new Point());

            _axisX.X1 = -bounds;
            _axisX.Y1 = 0;
            _axisX.X2 = bounds;
            _axisX.Y2 = 0;

            _axisY.X1 = 0;
            _axisY.Y1 = -bounds;
            _axisY.X2 = 0;
            _axisY.Y2 = bounds;

            _axisX.Stroke = Brushes.Black;
            _axisY.Stroke = Brushes.Black;
            _axisX.StrokeThickness = 2.0;
            _axisY.StrokeThickness = 2.0;

            window.WorldCanvas.Children.Add(_path);
            window.WorldCanvas.Children.Add(_axisX);
            window.WorldCanvas.Children.Add(_axisY);

            window.WorldCanvas.Children.Add(_zeroScale);

            foreach (var i in _axisXScale)
                window.WorldCanvas.Children.Add(i);

            foreach (var i in _axisYScale)
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

        private Path _path = new Path();
        private Line _axisX = new Line();
        private Line _axisY = new Line();
        private TextBlock _zeroScale;
        private List<TextBlock> _axisXScale = new List<TextBlock>();
        private List<TextBlock> _axisYScale = new List<TextBlock>();
    }

    public class ObjectListItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Func<string, DataTemplate> findResource = (key) => (container as FrameworkElement).FindResource(key) as DataTemplate;

            if (item is MovingObject)
            {
                return findResource("movingObjectDataTemplate");
            }
            else if (item is ElectricField)
            {
                return findResource("electricFieldDataTemplate");
            }
            else if (item is MagneticField)
            {
                return findResource("magneticFieldDataTemplate");
            }

            return base.SelectTemplate(item, container);
        }
    }
}
