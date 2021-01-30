using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSource.Tests
{
    [TestClass]
    public class RedisTests
    {
        [TestMethod]
        public void TestRedisCache()
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration: "localhost:6379");
            ICacheService cacheService = new RedisCacheService(connectionMultiplexer);
            string keyItemCache = "UnitTest";
            var result = cacheService.GetCacheValueAsync(keyItemCache).GetAwaiter().GetResult();
            Assert.IsNull(result);

            cacheService.SetCacheValueAsync(keyItemCache, "Some text here.").GetAwaiter();
         
            result = cacheService.GetCacheValueAsync(keyItemCache).GetAwaiter().GetResult();
            Assert.IsNotNull(result);
        }
    }
}
