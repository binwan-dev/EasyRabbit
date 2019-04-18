using System.Text;
using Newtonsoft.Json;

namespace Atlantis.Rabbit.Utilies
{
    public class DefaultSerializer: ISerializer
    {
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
