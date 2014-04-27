using PlaceModel;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ChipSynthesys.Common.Classes
{
    [Serializable]
    public class ChipTask
    {
        internal ChipTask()
        {
        }

        public ChipTask(Design design, PlacementGlobal approximate)
        {
            Design = design;
            Approximate = approximate;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public Design Design { get; internal set; }

        public PlacementGlobal Approximate { get; internal set; }

        public virtual void Save(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Create))
            {
                var stg = new BinaryFormatter();
                stg.Serialize(file, this);
            }
        }

        public static ChipTask Load(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open))
            {
                var stg = new BinaryFormatter();
                return stg.Deserialize(file) as ChipTask;
            }
        }
    }
}