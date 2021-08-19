using System;

namespace EasyRabbit.Utils
{
    public interface ISerializer
    {
        ReadOnlyMemory<byte> Serialize<T>(T message);

        T Deserialize<T>(ReadOnlyMemory<byte> data);
    }
}