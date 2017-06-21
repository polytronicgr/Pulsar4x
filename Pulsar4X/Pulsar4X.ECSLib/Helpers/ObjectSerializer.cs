using Newtonsoft.Json;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// This static helper class is used for seralizing and deseralising base objects that have derived types.
    /// so they can be sent via the messagepump, then deserialized into the correct derived type. 
    /// </summary>
    public static class ObjectSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};
        
        public static string SerializeObject(object objToSerilize)
        {
            return JsonConvert.SerializeObject(objToSerilize, Settings);
        }

        
        public static T DeserializeObject<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, Settings);
        }
    }
}