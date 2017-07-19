using CruPhysics.Shapes;
using CruPhysics.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CruPhysics
{
    public enum SelectionState
    {
        Normal,
        Hover,
        Select
    }

    public class Force : BindableVector
    {
        public Force()
        {

        }

        public Force(double x, double y)
            : base(x, y)
        {

        }

        public Force(Vector vector)
            : base(vector)
        {

        }
    }

    public class PhysicalObjectMetadata
    {
        public PhysicalObjectMetadata()
        {

        }

        public int ZIndex { get; set; }
        public int RunRank { get; set; }
    }

    public static class PhysicalObjectManager
    {
        static PhysicalObjectManager()
        {
            var assembly = Assembly.GetAssembly(typeof(PhysicalObjectManager));
            var types = assembly.GetExportedTypes();
            var physicalObjectTypes = from type in types
                                      where typeof(PhysicalObject).IsAssignableFrom(type) && !type.IsAbstract
                                      select type;
            foreach (var type in physicalObjectTypes)
            {
                Register(type.Name, (PhysicalObjectMetadata)type.GetField("Metadata", BindingFlags.IgnoreCase).GetValue(null));
            }
        }

        private static readonly Dictionary<string, PhysicalObjectMetadata> metadatas = new Dictionary<string, PhysicalObjectMetadata>();
        private static readonly SortedList<int, string> runRankList = new SortedList<int, string>();

        public static void Register(string name, PhysicalObjectMetadata metadata)
        {
            metadatas.Add(name, metadata);
            runRankList.Add(metadata.RunRank, name);
        }

        public static IList<string> GetOrderedByRunRank()
        {
            return runRankList.Values;
        }

        public static PhysicalObjectMetadata GetMetadata(string name)
        {
            return metadatas[name];
        }

        public static PhysicalObjectMetadata GetMetadata(this PhysicalObject physicalObejct)
        {
            return GetMetadata(physicalObejct.GetType().Name);
        }
    }

    public abstract class PhysicalObject : NotifyPropertyChangedObject
    {
        public const int SelectedZIndex = 1000;
        public const int ControllerZIndex = 1001;

        static PhysicalObject()
        {
            strokeBrushes.Add(SelectionState.Normal, Brushes.Black);
            strokeBrushes.Add(SelectionState.Hover, Brushes.Red);
            strokeBrushes.Add(SelectionState.Select, Brushes.Blue);

            strokeThickness.Add(SelectionState.Normal, 1.0);
            strokeThickness.Add(SelectionState.Hover, 2.0);
            strokeThickness.Add(SelectionState.Select, 2.0);
        }

        private static readonly Dictionary<SelectionState, Brush> strokeBrushes = new Dictionary<SelectionState, Brush>();
        private static readonly Dictionary<SelectionState, double> strokeThickness = new Dictionary<SelectionState, double>(); 

        public static IReadOnlyDictionary<SelectionState, Brush> StrokeBrushes => strokeBrushes;
        public static IReadOnlyDictionary<SelectionState, double> StrokeThickness => strokeThickness;


        private Scene scene = null;
        private string name = string.Empty;
        private SelectionState selectionState = SelectionState.Normal;
        private Color color;

        public PhysicalObject()
        {
            Color = Common.GetRamdomColor();
        }

        public Scene RelatedScene
        {
            get
            {
                return scene;
            }
            private set
            {
                scene = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => RelatedScene));
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Name));
            }
        }

        public SelectionState SelectionState
        {
            get
            {
                return selectionState;
            }
            set
            {
                selectionState = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => SelectionState));
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Color));
            }
        }

        public abstract void Run(Scene scene, TimeSpan time);
        public abstract void Move(Vector vector);
        public abstract Window CreatePropertyWindow();

        protected void PrepareShape(Shape shape)
        {
            shape.Cursor = Cursors.Arrow;
            shape.ContextMenu = (ContextMenu)Application.Current.FindResource("PhysicalObjectContextMenu");

            SetShowState(shape, SelectionState);
        }

        private void SetShowState(Shape shape, SelectionState selectionState)
        {
            shape.Stroke = StrokeBrushes[SelectionState];
            shape.StrokeThickness = StrokeThickness[SelectionState];
            if (selectionState == SelectionState.Select)
                Panel.SetZIndex(shape, SelectedZIndex);
            else
                Panel.SetZIndex(shape, this.GetMetadata().ZIndex);
        }

        internal void AddToScene(Scene scene)
        {
            OnAddToScene(scene);
        }

        internal void RemoveFromScene(Scene scene)
        {
            OnRemoveFromScene(scene);
        }

        protected virtual void OnAddToScene(Scene scene)
        {
            RelatedScene = scene;
            scene.PhysicalObjects.Add(this);
            scene.ClassifiedObjects[GetType().Name].Add(this);
        }

        protected virtual void OnRemoveFromScene(Scene scene)
        {
            RelatedScene = null;
            scene.PhysicalObjects.Remove(this);
            scene.ClassifiedObjects[GetType().Name].Remove(this);
        }
    }


    public class MovingObject : PhysicalObject
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 100, RunRank = 100 };

        private BindablePoint position = new BindablePoint();
        private double radius;
        private BindableVector velocity = new BindableVector();
        private double mass;
        private double charge;

        private List<Force> forces = new List<Force>();
        private MotionTrail trail = new MotionTrail();

        public MovingObject()
        {
            Name = "运动对象";
            Radius = 10.0;
            Mass = 1.0;
        }

        public BindablePoint Position => position;

        public double Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                RaisePropertyChangedEvent("Radius");
            }
        }

        public BindableVector Velocity => velocity;

        public double Mass
        {
            get
            {
                return mass;
            }
            set
            {
                mass = value;
                RaisePropertyChangedEvent("Mass");
            }
        }

        public double Charge
        {
            get
            {
                return charge;
            }
            set
            {
                charge = value;
                RaisePropertyChangedEvent("Charge");
            }
        }

        public override void Move(Vector vector)
        {
            Position.Move(vector);
        }

        public IList<Force> Forces => forces;

        public override Window CreatePropertyWindow()
        {
            return new MovingObjectPropertyDialog(this);
        }

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
            scene.BeginRunningEvent += OnBeginRunning;
            scene.RefreshEvent += OnRefresh;
            scene.AfterOneScanEvent += AfterOneScan;
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
            scene.BeginRunningEvent -= OnBeginRunning;
            scene.RefreshEvent -= OnRefresh;
            scene.AfterOneScanEvent -= AfterOneScan;
        }

        private void OnBeginRunning(object sender, EventArgs args)
        {
            StoreProperty();
            trail.AddPoint(Position);
        }

        private void OnRefresh(object sender, EventArgs args)
        {
            RecoverProperty();
            trail.Clear();
        }

        private void AfterOneScan(object sender, SceneScanEventArgs args)
        {
            trail.AddPoint(Position);
        }


        private Point _originalPosition;
        private Vector _originalVelocity;

        private void StoreProperty()
        {
            _originalPosition = Position;
            _originalVelocity = Velocity;
        }

        private void RecoverProperty()
        {
            Position.Set(_originalPosition);
            Velocity.Set(_originalVelocity);
        }

        public override void Run(Scene scene, TimeSpan time)
        {
            var t = time.TotalSeconds;
            var force = new Vector();
            foreach (var i in forces)
            {
                force.X += i.X;
                force.Y += i.Y;
            }
            var acceleration = force / Mass;
            Move((Vector)Velocity * t + acceleration * Math.Pow(t, 2) / 2.0);
            Velocity.Add(acceleration * t);
        }
    }



    public abstract class Field : PhysicalObject
    {
        private CruShape shape = new CruRectangle();

        protected Field()
        {

        }
        
        public CruShape Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => Shape));
            }
        }

        public override void Move(Vector vector)
        {
            Shape.Move(vector);
        }

        public bool IsMovingObjectInside(MovingObject movingObject)
        {
            return Shape.IsPointInside(movingObject.Position);
        }

        public void Influence(MovingObject movingObject, TimeSpan time)
        {
            if (IsMovingObjectInside(movingObject))
                CalculateEffect(movingObject, time);
        }

        public abstract void CalculateEffect(MovingObject movingObject, TimeSpan time);

        public override void Run(Scene scene, TimeSpan time)
        {
            foreach (MovingObject movingObject in scene.ClassifiedObjects[typeof(MovingObject).Name])
            {
                Influence(movingObject, time);
            }
        }

        protected override void OnAddToScene(Scene scene)
        {
            base.OnAddToScene(scene);
        }

        protected override void OnRemoveFromScene(Scene scene)
        {
            base.OnRemoveFromScene(scene);
        }
    }

    public class ElectricField : Field
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 50, RunRank = 50 };


        private BindableVector intensity = new BindableVector();

        public ElectricField()
        {
            Name = "电场";
        }

        public BindableVector Intensity
        {
            get
            {
                return intensity;
            }
        }

        public override void CalculateEffect(MovingObject movingObject, TimeSpan no_use)
        {
            movingObject.Forces.Add(new Force(
                    Intensity.X * movingObject.Charge,
                    Intensity.Y * movingObject.Charge
                ));
        }

        public override Window CreatePropertyWindow()
        {
            return new ElectricFieldPropertyDialog(this);
        }
    }

    public class MagneticField : Field
    {
        public static readonly PhysicalObjectMetadata metadata = new PhysicalObjectMetadata() { ZIndex = 50, RunRank = 49 };


        private double fluxDensity;

        public MagneticField()
        {
            Name = "磁场";
        }

        /// <summary>
        /// 为正时垂直纸面向里，为负时垂直于纸面向外
        /// </summary>
        public double FluxDensity
        {
            get
            {
                return fluxDensity;
            }
            set
            {
                fluxDensity = value;
                RaisePropertyChangedEvent(PropertyManager.GetPropertyName(() => FluxDensity));
            }
        }

        public override void CalculateEffect(MovingObject movingObject, TimeSpan time)
        {
            //qvB = mvω => ω = qB/m => α = ωt = qBt/m
            movingObject.Velocity.Set(Common.Rotate(movingObject.Velocity,
                -time.TotalSeconds * movingObject.Charge * FluxDensity / movingObject.Mass));
        }

        public override Window CreatePropertyWindow()
        {
            return new MagneticFieldPropertyDialog(this);
        }
    }
}
