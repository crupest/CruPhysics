namespace CruPhysics.Shapes
{
    public interface ICircle : IShape
    {
        BindablePoint Center { get; }
        double Radius { get; set; }
    }
}
