using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CruPhysics
{
    class MotionTrail : NotifyPropertyChangedObject
    {
        private PathFigure pathFigure = null;
        private PathGeometry geometry = new PathGeometry();
        private Path shape = new Path();
        private SelectionState selectionState = SelectionState.Normal;
        private SolidColorBrush brush = new SolidColorBrush(Common.GetRamdomColor());

        public MotionTrail()
        {
            shape.Data = geometry;
            shape.Stroke = brush;
            shape.StrokeThickness = 1.0;
            shape.Cursor = Cursors.Arrow;
            shape.MouseEnter += (sender, args) =>
            {
                if (SelectionState == SelectionState.Select)
                    return;
                SelectionState = SelectionState.Hover;
            };
            shape.MouseLeave += (sender, args) =>
            {
                if (SelectionState == SelectionState.Select)
                    return;
                SelectionState = SelectionState.Normal;
            };
            shape.MouseDown += (sender, args) =>
            {
                SelectionState = SelectionState.Select;
            };
            PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SelectionState")
                {
                    switch (SelectionState)
                    {
                        case SelectionState.Normal:
                            shape.StrokeThickness = 1.0;
                            break;
                        case SelectionState.Hover:
                            shape.StrokeThickness = 2.0;
                            break;
                        case SelectionState.Select:
                            shape.StrokeThickness = 2.0;
                            break;
                    }
                }
            };
        }

        public void AddPoint(Point point)
        {
            point = Common.TransformPoint(point);
            if (pathFigure == null)
            {
                pathFigure = new PathFigure()
                {
                    StartPoint = point,
                    IsClosed = false
                };
                geometry.Figures.Add(pathFigure);
            }
            else
            {
                pathFigure.Segments.Add(new LineSegment(point, true));
            }
        }

        public void Clear()
        {
            if (pathFigure != null)
            {
                geometry.Figures.Remove(pathFigure);
                pathFigure = null;
            }
        }

        public Geometry Geometry => geometry;

        public Path Shape => shape;

        public SelectionState SelectionState
        {
            get
            {
                return selectionState;
            }
            set
            {
                selectionState = value;
                RaisePropertyChangedEvent("SelectionState");
            }
        }
    }
}
