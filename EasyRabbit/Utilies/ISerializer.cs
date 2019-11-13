using System.Text;

namespace EasyRabbit.Utilies
{
    public interface ISerializer
    {
        byte[] Serialize<T>(T data);

        string SerializeStr<T>(T data);

        T Deserialize<T>(byte[] data);
    }
}
