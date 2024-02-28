using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyServer.src.Core.Data
{
    public static class LocalDataManager
    {
        private static string json;
        public static string path;

        static LocalDataManager()
        {
            if (string.IsNullOrEmpty(path))
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                path = currentDirectory + "\\LocalData.Data";
            }
            if (!File.Exists(path))
            {
                FileStream fs = File.Create(path);
                fs.Close();
            }
            json = File.ReadAllText(path);
        }

        public static void ClearData()
        {
            json = string.Empty;
            File.WriteAllText(path, json);
        }

        public static void AddData(string id, object o)
        {

            JObject jsonObj = new JObject();
            if (json != "")
                jsonObj = JObject.Parse(json);
            if (jsonObj.ContainsKey(id))
                return;
            jsonObj.Add(id, JToken.FromObject(o));
            json = jsonObj.ToString();
            File.WriteAllText(path, json);
        }

        public static void RemoveData(string id)
        {
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj.Remove(id);
            json = JsonConvert.SerializeObject(jsonObj);
            File.WriteAllText(path, json);
        }

        public static T2 QueryData<T1, T2>(string id, string fieldName)
        {
            T2 t2 = default;
            if (!string.IsNullOrEmpty(json))
            {
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                if (!jsonObj.ContainsKey(id))
                    return t2;

                T1 t1 = JsonConvert.DeserializeObject<T1>(jsonObj[id].ToString());
                FieldInfo[] fi = typeof(T1).GetFields();
                foreach (var f in fi)
                    if (f.Name == fieldName)
                        return (T2)f.GetValue(t1);
            }
            return t2;
        }

        public static void UpdateData(string id, object o)
        {
            JObject jsonObj;
            if (!string.IsNullOrEmpty(json))
            {
                jsonObj = JObject.Parse(json);
                if (jsonObj[id] != null)
                {
                    jsonObj[id] = JToken.FromObject(o);
                }
                else
                {
                    jsonObj.Add(id, JToken.FromObject(o));
                }
            }
            else
            {
                jsonObj = new JObject
                {
                    { id, JToken.FromObject(o) }
                };

            }
            json = jsonObj.ToString();
            File.WriteAllText(path, json);
        }
    }
}
