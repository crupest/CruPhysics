using System.Collections.Generic;

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
