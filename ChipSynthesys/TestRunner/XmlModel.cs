using System;
using ChipSynthesys.Common.Classes;

namespace TestRunner
{
    [Serializable]
    public class XmlModel : XmlStorageHelper<XmlModel>
    {
        public void Save(string name)
        {
            Store(name,this);
        }
    }
}