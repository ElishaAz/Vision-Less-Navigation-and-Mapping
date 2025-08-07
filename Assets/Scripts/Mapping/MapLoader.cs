using System.IO;
using Mapping.Data;
using UnityEngine;

namespace Mapping
{
    public static class MapLoader
    {
        public static Map Load(string path)
        {
            var json = File.ReadAllText(path);
            Debug.Log(json);
            // return Newtonsoft.Json.JsonConvert.DeserializeObject<Map>(json);
            return JsonUtility.FromJson<Map>(json);
        }

        public static void Save(Map map, string path)
        {
            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(map);
            var json = JsonUtility.ToJson(map);
            Debug.Log(json);
            File.WriteAllText(path, json);
        }
    }
}