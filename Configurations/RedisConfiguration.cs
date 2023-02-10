using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisCache.Configurations
{
    public class RedisConfiguration
    {
        public bool Enable { get; set; }

        public string? ConnectionString { get; set; }
    }
}