using System;
using EasyRabbit.Utils;
using Newtonsoft.Json;

namespace EasyRabbit.Extensions.Newtonsoft.Json
{
    public class JsonSerializer : ISerializer
    {
        public T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var strData = System.Text.Encoding.UTF8.GetString(data.ToArray());
            return JsonConvert.DeserializeObject<T>(strData);
        }

        public ReadOnlyMemory<byte> Serialize<T>(T message)
        {
            var strData = JsonConvert.SerializeObject(message);
            return System.Text.Encoding.UTF8.GetBytes(strData);
        }
    }
}