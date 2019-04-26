using System;
using System.Collections.Generic;
using System.Reflection;
using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.Options;

namespace Atlantis.Rabbit
{
    public class RabbitBuilder
    {
        internal static IServiceProvider _serviceProvider;

        public RabbitBuilder()
        {
            Metadatas=new List<Type>();
            JsonSerializer=new DefaultSerializer();
        }
        
        public RabbitServerSetting ServerOptions{get;set;}

        public string[] ScanAssemblies{get;set;}

        public ISerializer JsonSerializer{get;set;}

        public ISerializer BinarySerializer{get;set;}

        internal IList<Type> Metadatas {get;set;}

        internal static IServiceProvider ServiceProvider
        {
            get
            {
                if(_serviceProvider==null)
                {
                    throw new InvalidOperationException("Please call UseRabbit Method with IServiceProvider");
                }
                return _serviceProvider;
            }
            set
            {
                _serviceProvider=value;
            }
        }

     }
}
