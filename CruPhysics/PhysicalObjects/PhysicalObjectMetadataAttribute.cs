using System;

namespace CruPhysics.PhysicalObjects
{
    [AttributeUsage(AttributeTargets.Class)]
    class PhysicalObjectMetadataAttribute : Attribute
    {
        public int ZIndex { get; set; }
        public int RunRank { get; set; }
        public Type ViewType { get; set; }
    }
}
