using UnityEngine;
using System.Collections;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Utils
{
    //Extension class to provide serialize / deserialize methods to object.
    //src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
    //NOTE: You need add [Serializable] attribute in your class to enable serialization
    public static class ObjectSerializationExtension
    {

        public static byte[] SerializeToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(this byte[] byteArray) where T : class
        {
            if (byteArray == null)
            {
                return null;
            }
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }

        // A helper function to convert a color32 array to a byte array 
        public static byte[] Color32ArrayToByteArray(Color32[] colors)
        {
            byte[] bytes = new byte[colors.Length * 4];
            for (int i = 0; i < bytes.Length / 4; i += 4)
            {
                bytes[i] = colors[i].r;
                bytes[i + 1] = colors[i].g;
                bytes[i + 2] = colors[i].b;
                bytes[i + 3] = colors[i].a;
            }
            return bytes;
        }


        // A helper function to convert a JSON string to a byte array
        public static byte[] JsonStringToByteArray(string jsonString)
        {
            var encoding = new UTF8Encoding();
            return encoding.GetBytes(jsonString.Substring(1, jsonString.Length - 2));
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }



    }
}
