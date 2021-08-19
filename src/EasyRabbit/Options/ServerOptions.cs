namespace EasyRabbit.Options
{
    public class ServerOptions
    {

        public ServerOptions()
        {
            RequestedHeartbeat = 60;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public ushort RequestedHeartbeat { get; set; }

        public string VirtualHost { get; set; }

        public static ServerOptions Default { get; set; }
    }
}