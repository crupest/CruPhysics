using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CruPhysics.PhysicalObjects
{
    public class PhysicalObjectMetadata
    {
        public int ZIndex { get; set; }
        public int RunRank { get; set; }
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
                var field = type.GetField("metadata", BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
                if (field == null)
                    throw new Exception("Can't find the metadata of " + type.FullName);
                var metadata = field.GetValue(null) as PhysicalObjectMetadata;
                Register(type.Name, metadata);
            }
        }

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
}
