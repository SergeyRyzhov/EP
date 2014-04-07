using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PlaceModel;

namespace ChipSynthesys.Common.Classes
{
    [Serializable]
    public class ChipTaskResult : ChipTask
    {
        public ChipTaskResult(Design design, PlacementGlobal approximate, PlacementDetail detail) : base(design, approximate)
        {
            Detail = detail;
        }

        public PlacementDetail Detail { get; internal set; }

        public override void Save(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Create))
            {
                var stg = new BinaryFormatter();
                stg.Serialize(file, this);
            }
        }

        public new static ChipTaskResult Load(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open))
            {
                var stg = new BinaryFormatter();
                return stg.Deserialize(file) as ChipTaskResult;
            }
        }
    }
}
