using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CruPhysics.PhysicalObjects
{
    public class PhysicalObjectMetadata
    {
        public int ZIndex { get; internal set; }
        public int RunRank { get; internal set; }
        public Type ViewType { get; internal set; }
    }

    public static class PhysicalObjectManager
    {
        private static readonly Dictionary<string, PhysicalObjectMetadata> metadatas = new Dictionary<string, PhysicalObjectMetadata>();
        private static readonly SortedList<int, string> runRankList = new SortedList<int, string>();

        public static void ScanPhysicalObjectType(Assembly assembly)
        {
            var types = assembly.GetExportedTypes();
            var physicalObjectTypes = from type in types
                where typeof(PhysicalObject).IsAssignableFrom(type) && !type.IsAbstract
                select type;
            foreach (var type in physicalObjectTypes)
            {
                Register(type);
            }
        }

        private static PhysicalObjectMetadata AttributeToMetadata(PhysicalObjectMetadataAttribute attribute)
        {
            return new PhysicalObjectMetadata
            {
                ZIndex = attribute.ZIndex,
                RunRank = attribute.RunRank,
                ViewType = attribute.ViewType
            };
        }

        public static void Register(Type type)
        {
            var name = type.Name;
            PhysicalObjectMetadataAttribute attribute;
            try
            {
                attribute =
                    Attribute.GetCustomAttribute(type, typeof(PhysicalObjectMetadataAttribute)) as
                        PhysicalObjectMetadataAttribute;
            }
            catch (Exception e)
            {
                throw new Exception("Can't get PhysicalObjectMetadataAttribute for " + type.FullName, e);
            }
            var metadata = AttributeToMetadata(attribute);

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
}
