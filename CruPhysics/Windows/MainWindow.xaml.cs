﻿using System;
using CruPhysics.PhysicalObjects;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CruPhysics.Controls;
using CruPhysics.PhysicalObjects.Views;
using CruPhysics.ViewModels;

namespace CruPhysics.Windows
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand NewMovingObject   = new RoutedUICommand("运动对象(_0)",  "moving_object",    typeof(MainWindow));
        public static readonly RoutedUICommand NewElectricField  = new RoutedUICommand("电场(_E)",     "electric_field",   typeof(MainWindow));
        public static readonly RoutedUICommand NewMagneticField  = new RoutedUICommand("磁场(_M)",     "magnetic_field",   typeof(MainWindow));
        public static readonly RoutedUICommand Property          = new RoutedUICommand("属性(_P)",     "property",         typeof(MainWindow));
        public static readonly RoutedUICommand Delete            = new RoutedUICommand("删除(_D)",     "delete",           typeof(MainWindow));
        public static readonly RoutedUICommand Begin             = new RoutedUICommand("开始(_B)",     "begin",            typeof(MainWindow));
        public static readonly RoutedUICommand Stop              = new RoutedUICommand("停止(_S)",     "stop",             typeof(MainWindow));
        public static readonly RoutedUICommand Restart           = new RoutedUICommand("重新(_R)",     "restart",          typeof(MainWindow));
        public static readonly RoutedUICommand ResetView         = new RoutedUICommand("重置视图(_R)",  "reset_view",       typeof(MainWindow));



        // ReSharper disable once NotAccessedField.Local
        private CoordinateSystem coordinateSystem;

        private readonly IDictionary<PhysicalObject, UIElement> physicalObjectViewMap = new Dictionary<PhysicalObject, UIElement>();
        private readonly IDictionary<MovingObject, MotionTrailView> motionTrailViewMap = new Dictionary<MovingObject, MotionTrailView>();

        private IEnumerable<ControllerView> controllerViews;

        public MainWindow()
        {
            InitializeComponent();

            ViewModel = (MainViewModel)FindResource("ViewModel");

            coordinateSystem = new CoordinateSystem(this);

            ViewModel.Scene.PropertyChanged += SceneOnPropertyChanged;
            ViewModel.Scene.PhysicalObjects.CollectionChanged += PhysicalObjects_CollectionChanged;
            ViewModel.Scene.ClassifiedObjects[typeof(MovingObject).Name].CollectionChanged += MovingObjectOnCollectionChanged;
            ObjectList.Focus();
        }

        private void MovingObjectOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (MovingObject movingObject in args.OldItems)
                {
                    WorldCanvas.Children.Remove(motionTrailViewMap[movingObject]);
                    motionTrailViewMap.Remove(movingObject);
                }
            }

            if (args.NewItems != null)
            {
                foreach (MovingObject movingObject in args.NewItems)
                {
                    var motionTrailView = new MotionTrailView() {DataContext = movingObject.MotionTrail};
                    WorldCanvas.Children.Add(motionTrailView);
                    motionTrailViewMap.Add(movingObject, motionTrailView);
                }
            }
        }

        private void SceneOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var scene = (Scene) sender;
            if (propertyChangedEventArgs.PropertyName == nameof(Scene.SelectionBox))
            {
                if (controllerViews != null)
                {
                    foreach (var controllerView in controllerViews)
                    {
                        WorldCanvas.Children.Remove(controllerView);
                    }
                    controllerViews = null;
                }

                if (scene.SelectionBox != null)
                {
                    var controllerViewList = new List<ControllerView>();
                    foreach (var controller in scene.SelectionBox.Controllers)
                    {
                        var controllerView = new ControllerView() {DataContext = controller};
                        controllerViewList.Add(controllerView);
                        WorldCanvas.Children.Add(controllerView);
                    }
                    controllerViews = controllerViewList;
                }
            }
        }

        private void PhysicalObjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (PhysicalObject i in args.OldItems)
                {
                    var uiElement = physicalObjectViewMap[i];
                    if (uiElement != null)
                        WorldCanvas.Children.Remove(uiElement);

                    physicalObjectViewMap.Remove(i);
                }
            }

            if (args.NewItems != null)
            {
                foreach (PhysicalObject i in args.NewItems)
                {
                    var viewType = PhysicalObjectManager.GetMetadata(i.GetType().Name).ViewType;
                    FrameworkElement uiElement = null;

                    if (viewType != null)
                    {
                        try
                        {
                            uiElement = (FrameworkElement) Activator.CreateInstance(viewType);
                            uiElement.DataContext = i;
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Can't create view instance for " + i.GetType().Name + ".", e);
                        }
                    }

                    if (uiElement != null)
                        WorldCanvas.Children.Add(uiElement);

                    physicalObjectViewMap.Add(i, uiElement);
                }
            }
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

        private Point previousMousePosition;

        private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = (IInputElement) sender;
            ViewModel.Scene.OnMouseDown();
            canvas.CaptureMouse();
            previousMousePosition = e.GetPosition(canvas);
        }

        public UIElement GetPhysicalObjectView(PhysicalObject o)
        {
            return physicalObjectViewMap[o];
        }

        private void MainCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var canvas = (IInputElement) sender;
            canvas.ReleaseMouseCapture();
        }

        private void MainCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var canvas = (IInputElement) sender;
            if (canvas.IsMouseCaptured)
            {
                var presentPosition = e.GetPosition(canvas);
                var vector = presentPosition - previousMousePosition;
                var matrix = WorldCanvas.RenderTransform.Value;
                matrix.Translate(vector.X, vector.Y);
                WorldCanvas.RenderTransform = new MatrixTransform(matrix);
                previousMousePosition = presentPosition;
            }
        }
    }


    public class CoordinateSystem
    {
        private const double graduation = 50.0;

        private readonly Brush axisBrush = Brushes.Black;
        private readonly Brush nonAxisLineBrush = Brushes.Gray;

        // ReSharper disable CollectionNeverQueried.Local
        private readonly IList<Line> horizentalLines = new List<Line>();
        private readonly IList<Line> verticalLines = new List<Line>();
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly Line axisX;
        private readonly Line axisY;
        private readonly TextBlock zeroScale;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        private readonly IList<TextBlock> xScales = new List<TextBlock>();
        private readonly IList<TextBlock> yScales = new List<TextBlock>();
        // ReSharper restore CollectionNeverQueried.Local


        public CoordinateSystem(MainWindow window)
        {
            var bounds = window.ViewModel.Scene.Bounds;
            var children = window.WorldCanvas.Children;


            TextBlock CreateGraduation(double value, Point position)
            {
                var textBlock = new TextBlock
                {
                    Text = value.ToString(),
                    Padding = new Thickness(2.0)
                };
                WorldCanvas.SetLeft(textBlock, position.X);
                WorldCanvas.SetTop(textBlock, position.Y);
                return textBlock;
            }

            Line CreateHorizontalLine(double y, Brush stroke, double thickness = 1.0)
            {
                return new Line {X1 = -bounds, X2 = bounds, Y1 = -y, Y2 = -y, Stroke = stroke, StrokeThickness = thickness};
            }

            void CreateHorizontalLineAndScale(double y)
            {
                var line = CreateHorizontalLine(y, nonAxisLineBrush);
                horizentalLines.Add(line);
                children.Add(line);

                var scale = CreateGraduation(y, new Point(0.0, y));
                yScales.Add(scale);
                children.Add(scale);
            }

            Line CreateVerticalLine(double x, Brush stroke, double thickness = 1.0)
            {
                return new Line {X1 = x, X2 = x, Y1 = bounds, Y2 = -bounds, Stroke = stroke, StrokeThickness = thickness};
            }

            void CreateVerticalLineAndScale(double x)
            {
                var line = CreateVerticalLine(x, nonAxisLineBrush);
                verticalLines.Add(line);
                children.Add(line);

                var scale = CreateGraduation(x, new Point(x, 0.0));
                xScales.Add(scale);
                children.Add(scale);
            }

            for (var i = -graduation; i > -bounds; i -= graduation)
            {
                CreateHorizontalLineAndScale(i);
                CreateVerticalLineAndScale(i);
            }

            for (var i = graduation; i < bounds; i += graduation)
            {
                CreateHorizontalLineAndScale(i);
                CreateVerticalLineAndScale(i);
            }


            axisX = CreateHorizontalLine(0.0, axisBrush, 2.0);
            children.Add(axisX);

            axisY = CreateVerticalLine(0.0, axisBrush, 2.0);
            children.Add(axisY);

            zeroScale = CreateGraduation(0.0, new Point());
            children.Add(zeroScale);
        }
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
