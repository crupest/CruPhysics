using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CruPhysics
{
    class MotionTrail
    {
        private PathFigure pathFigure = null;
        private PathGeometry geometry = new PathGeometry();

        public MotionTrail()
        {

        }

        public void AddPoint(Point point)
        {
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

        public Geometry Geometry
        {
            get
            {
                return geometry;
            }
        }
    }
}
