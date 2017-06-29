using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CruPhysics
{
    class MotionTrail
    {
        private PathFigure pathFigure = null;
        private PathGeometry geometry = new PathGeometry();
        private Path shape = new Path();

        public MotionTrail()
        {
            shape.Data = geometry;
            shape.Stroke = new SolidColorBrush(Common.GetRamdomColor());
            shape.StrokeThickness = 1.0;
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
    }
}
