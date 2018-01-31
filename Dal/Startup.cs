using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Dal
{
    public class Startup
    {
        public struct Credentials
        {
            public string DBUID;
            public string DBHost;
            public string DBPort;
            public string DBPassword;
            public string DBConnectionString;
            public string RedisHost;
            public string RedisPort;
            public string RedisPassword;
        }

        public static Credentials ParseVCAP()
        {
            var redisSessionStateProvider = new RedisSessionStateProvider();
            Credentials result = new Credentials();
            //{ "p-redis": [{ "credentials":{"host": "127.0.0.1","port": "1","password": "HelloWorld", "DBUID": "HelloWorld",  "DBHost": "HelloWorld",  "DBPort": "HelloWorld",  "DBPassword": "HelloWorld",
            //  "DBConnectionString": "HelloWorld",  "RedisHost": "HelloWorld", "RedisPort": "HelloWorld", "RedisPassword": "HelloWorld"}}]}

            const string dbServiceName = "mssql-dev";
            const string redisServiceName = "p-redis";
            string vcapServices = Environment.GetEnvironmentVariable("VCAP_SERVICES");

            // if we are in the cloud and DB service was bound successfully...
            if (vcapServices != null)
            {
                dynamic json = JsonConvert.DeserializeObject(vcapServices);
                foreach (dynamic obj in json.Children())
                {

                    switch (((string)obj.Name).ToLowerInvariant())
                    {
                        case dbServiceName:
                            {
                                dynamic credentials = (((JProperty)obj).Value[0] as dynamic).credentials;
                                result.DBHost = credentials != null ? credentials.host : null;
                                result.DBPort = credentials != null ? credentials.port : null;
                                result.DBUID = credentials != null ? credentials.username : null;
                                result.DBPassword = credentials != null ? credentials.password : null;
                                result.DBConnectionString = credentials != null ? credentials.connectionString : null;
                            }
                            break;

                        case redisServiceName:
                            {
                                dynamic credentials = (((JProperty)obj).Value[0] as dynamic).credentials;
                                result.RedisHost = credentials?.host;
                                result.RedisPort = credentials?.port;
                                result.RedisPassword = credentials?.password;
                            }
                            break;

                        default:
                            break;
                    }

                }
            }
            return result;
        }

        public static string GetRedisConnectionString()
        {
            var settings = ParseVCAP();
            if (null != settings.RedisPassword)
            {
                ConfigurationOptions config = new ConfigurationOptions
                {
                    EndPoints =
                    {
                        { settings.RedisHost }//, int.Parse(settings.RedisPort)}
                    },
                    CommandMap = CommandMap.Create(new HashSet<string>
                    { // EXCLUDE a few commands
                      /*"INFO", "CONFIG", "CLUSTER",*/
                      "PING", "ECHO",
                    }, available: false),
                    KeepAlive = 180,
                    DefaultVersion = new Version(2, 8, 8),
                    Password = settings.RedisPassword
                };
                return config.ToString();
            }
            return null;
        }

    }
}
