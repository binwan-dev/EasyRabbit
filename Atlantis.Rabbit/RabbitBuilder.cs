using System;
using System.Collections.Generic;
using System.Reflection;
using Atlantis.Rabbit.Utilies;
using Microsoft.Extensions.Options;

namespace Atlantis.Rabbit
{
    public class RabbitBuilder
    {
        public RabbitBuilder()
        {
            Metadatas=new List<Type>();
            Serializer=new DefaultSerializer();
        }
        
        public RabbitServerSetting ServerOptions{get;set;}

        public string[] ScanAssemblies{get;set;}

        public ISerializer Serializer{get;set;}

        internal IList<Type> Metadatas {get;set;}

     }
}
