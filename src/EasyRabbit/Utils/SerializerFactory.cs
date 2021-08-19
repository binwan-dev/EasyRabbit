namespace EasyRabbit.Utils
{
    public class SerializeFactory
    {
        public static ISerializer Serializer { get; private set; }

        public static void RegisterSerializer(ISerializer serializer)
        {
            Serializer = serializer;
        }
    }
}