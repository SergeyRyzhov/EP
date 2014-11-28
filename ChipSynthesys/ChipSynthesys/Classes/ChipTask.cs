using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using PlaceModel;

namespace ChipSynthesys.Common.Classes
{
    [Serializable]
    public class ChipTask
    {
        public ChipTask(Design design, PlacementGlobal globalPlacement)
        {
            Design = design;
            this.GlobalPlacement = globalPlacement;
        }

        public ChipTask(Design design, PlacementGlobal globalPlacement, PlacementDetail currentPlacement)
            : this(design, globalPlacement)
        {
            CurrentPlacement = currentPlacement;
        }

        internal ChipTask()
        {
        }

        public Design Design { get; internal set; }

        public PlacementGlobal GlobalPlacement { get; internal set; }

        public PlacementDetail CurrentPlacement { get; internal set; }

        public static ChipTask Load(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open))
            {
                var stg = new BinaryFormatter();
                return stg.Deserialize(file) as ChipTask;
            }
        }

        public virtual void Save(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Create))
            {
                var stg = new BinaryFormatter();
                stg.Serialize(file, this);
            }
        }
    }
}