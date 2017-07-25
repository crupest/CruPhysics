using System.Windows;
using System.Windows.Media;

namespace CruPhysics.PhysicalObjects
{
    public class MotionTrail : NotifyPropertyChangedObject
    {
        private PathFigure pathFigure;
        private readonly PathGeometry geometry = new PathGeometry();
        private Color color = Common.GetRamdomColor();
        private double strokeThickness = 1.0;

        public MotionTrail()
        {

        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                RaisePropertyChangedEvent(nameof(Color));
            }
        }

        public double StrokeThickness
        {
            get => strokeThickness;
            set
            {
                strokeThickness = value;
                RaisePropertyChangedEvent(nameof(StrokeThickness));
            }
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
    }
}
