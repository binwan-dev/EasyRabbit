using System.Text;
using Newtonsoft.Json;

namespace EasyRabbit.Utilies
{
    public class DefaultSerializer: ISerializer
    {
        public string SerializeStr<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(byte[] data)
        {
            var str=Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public byte[] Serialize<T>(T data)
        {
            var str=JsonConvert.SerializeObject(data);
            return Encoding.UTF8.GetBytes(str);
        }
    }
}
