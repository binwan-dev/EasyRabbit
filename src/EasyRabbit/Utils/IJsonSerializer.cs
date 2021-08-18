using System;

namespace EasyRabbit.Utils
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(ReadOnlyMemory<byte> data);
    }
}