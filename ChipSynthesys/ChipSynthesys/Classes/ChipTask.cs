using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using PlaceModel;

namespace ChipSynthesys.Common.Classes
{
    [Serializable]
    public class ChipTask
    {
        public ChipTask(string name, Design design, PlacementGlobal globalPlacement)
        {
            Name = name;
            Design = design;
            this.GlobalPlacement = globalPlacement;
        }

        public ChipTask(string name, Design design, PlacementGlobal globalPlacement, PlacementDetail currentPlacement)
            : this(name, design, globalPlacement)
        {
            CurrentPlacement = currentPlacement;
        }

        internal ChipTask()
        {
        }

        public string Name { get; internal set; }

        public Design Design { get; internal set; }

        public PlacementGlobal GlobalPlacement { get; internal set; }

        public PlacementDetail CurrentPlacement { get; set; }

        public static ChipTask Load(string fileName)
        {
            using (var file = File.Open(fileName, FileMode.Open))
            {
                var stg = new BinaryFormatter();
                var chipTask = stg.Deserialize(file) as ChipTask;
                if (chipTask != null)
                {
                    chipTask.Name = Path.GetFileNameWithoutExtension(fileName);
                }

                return chipTask;
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

        public ChipTask Clone()
        {
            return this.MemberwiseClone() as ChipTask;
        }
    }
}