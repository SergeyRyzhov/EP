using System.IO;
using System.Runtime.Serialization.Formatters.Soap;
using System.Xml.Serialization;

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

    public class XmlStorageHelper<T> where T : class
    {
        private readonly XmlSerializer m_serializer;

        public XmlStorageHelper()
        {
            m_serializer = new XmlSerializer(GetType());
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