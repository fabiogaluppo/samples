//Sample provided by Fabio Galuppo
//August 2013

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace SupportLibrary
{
    public static class Cloning
    {
        public static class BySerialization
        {
            public static T DeepCopy<T>(T instance)
            {
                var serializer = GetSerializer<T>();
                return DeepCopyDeserialize<T>(DeepCopySerialize(instance, serializer), serializer);
            }

            private static DataContractSerializer GetSerializer<T>()
            {
                return new DataContractSerializer(typeof(T));
            }

            private static MemoryStream DeepCopySerialize<T>(T instance, DataContractSerializer serializer)
            {
                var serializationStream = new MemoryStream();
                using(var xmlWriter = XmlDictionaryWriter.CreateBinaryWriter(serializationStream))
                    serializer.WriteObject(xmlWriter, instance);
                return serializationStream;
            }

            private static T DeepCopyDeserialize<T>(MemoryStream stream, DataContractSerializer serializer)
            {
                var deserializationStream = new MemoryStream(stream.ToArray());
                var instance = default(T);
                using(var xmlReader = XmlDictionaryReader.CreateBinaryReader(deserializationStream, XmlDictionaryReaderQuotas.Max))
                    instance = (T)serializer.ReadObject(xmlReader);
                return instance;
            }
        }
    }
}