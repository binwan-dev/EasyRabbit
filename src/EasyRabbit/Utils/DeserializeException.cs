using System;

namespace EasyRabbit.Utils
{
    public class DeserializeException : Exception
    {
        public DeserializeException(string message, Exception innerException) : base(message, innerException)
        { }
    }
}