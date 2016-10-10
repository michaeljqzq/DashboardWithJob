using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using MSDNDashboard.Util;

namespace MSDNDashboardLibrary
{
    public class RedisConnector
    {
        private RedisClient client;

        public RedisConnector()
        {
            client = new RedisClient(EncryptionHelper.Configs["BlogsRedisUri"]);
            client.Auth(EncryptionHelper.Configs["BlogsRedisAuthKey"]);
        }

        public bool RemoveSiteOptionCache(int blogId)
        {
            string[] keys = {"{0}:options:alloptions","{0}:options:notoptions"};
            for(int i = 0;i<keys.Length;i++)
            {
                keys[i] = string.Format(keys[i], blogId);
            }
            var result = client.Del(keys);
            return result == keys.Length;
        }
    }
}
