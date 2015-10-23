using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Infrastructure
{
    public static class JsonExentions
    {
        public static string ToJson(this object objToSerialize)
        {
            return JsonConvert.SerializeObject(objToSerialize, Formatting.Indented);
        }

        public static T FromJson<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        public static void SaveToUserProfile(this object objToSerialize, string fileName)
        {
            var filePath = SerializationFilePath(fileName);
            SaveToFile(objToSerialize, filePath);
        }

        public static void SaveToFile(this object objToSerialize, string filePath)
        {
            var directoryInfo = new FileInfo(filePath).Directory;
            if (directoryInfo != null && !directoryInfo.Exists) directoryInfo.Create();
            File.WriteAllText(filePath, objToSerialize.ToJson());
        }

        public static T LoadFromUserProfile<T>(this string fileName)
        {
            var filePath = SerializationFilePath(fileName);

            return LoadFromFile<T>(filePath);
        }

        public static T LoadFromFile<T>(this string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists == false) return default(T);
            return File.ReadAllText(filePath).FromJson<T>();
        }

        private static string SerializationFilePath(string fileName)
        {
            var assemblyName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            assemblyName = new FileInfo(assemblyName).Name.Split('.').First();
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName,
                fileName);
            return filePath;
        }
    }
}