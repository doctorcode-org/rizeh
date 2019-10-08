using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Parsnet
{
    public static class Serializer
    {
        public static object DeserializeObject<T>(this string toDeserialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new PreMergeToMergedDeserializationBinder();
            MemoryStream ms = new MemoryStream();
            var data = Convert.FromBase64String(toDeserialize);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            return bf.Deserialize(ms);
        }

        public static string SerializeObject<T>(this T toSerialize)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, toSerialize);
            return Convert.ToBase64String(ms.ToArray());
        }
    }

    sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            String exeAssembly = Assembly.GetExecutingAssembly().FullName;
            string t = "RizehClient.Products";

            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}", t, exeAssembly));

            return typeToDeserialize;
        }
    }
}
