using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDNDashboard.Util;
using StackExchange.Redis;

namespace MSDNDashboardLibrary
{
    public class RedisConnector
    {
        private ConnectionMultiplexer client;

        public RedisConnector()
        {
            client =
                ConnectionMultiplexer.Connect(EncryptionHelper.Configs["BlogsRedisUri"] + ",password=" +
                                              EncryptionHelper.Configs["BlogsRedisAuthKey"]);
        }

        public bool RemoveSiteOptionCache(int blogId)
        {
            string[] keys = {"{0}:options:alloptions","{0}:options:notoptions"};
            var db = client.GetDatabase();
            for(int i = 0;i<keys.Length;i++)
            {
                if (!db.KeyDelete(string.Format(keys[i], blogId)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
