using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace ChipSynthesys.Common.Classes
{
    internal class StorageHelper<T> where T : class
    {
        private readonly SoapFormatter m_serializer;

        public StorageHelper()
        {
            m_serializer = new SoapFormatter();
        }

        public void Store(string fileName, T obj)
        {
            using (var fs = File.Open(fileName, FileMode.Create))
            {
                m_serializer.Serialize(fs, obj);
            }
        }

        public T Load(string fileName)
        {
            using (var fs = File.Open(fileName, FileMode.Open))
            {
                return m_serializer.Deserialize(fs) as T;
            }
        }
    }
}