﻿using System;
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

namespace CruPhysics
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
        

        private MainViewModel viewModel;
        private CoordinateSystem coordinateSystem;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel(this);

            coordinateSystem = new CoordinateSystem(this);

            Focus();
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
            PhysicalObject.SelectedObject.CreatePropertyWindow().ShowDialog();
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            viewModel.Scene.Remove(PhysicalObject.SelectedObject);
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PhysicalObject.SelectedObject != null;
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
            Canvas.SetLeft(WorldCanvas, MainCanvas.ActualWidth / 2.0);
            Canvas.SetTop(WorldCanvas, MainCanvas.ActualHeight / 2.0);
        }

        private void MainCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PhysicalObject.SelectedObject = null;
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
}
