using System;
using System.Text;
using System.Linq;
using Xunit;

namespace DnsTest
{
    using Dns;

    public class DnsCacheTests
    {
        [Fact]
        public void Test1() {
            Dns.Contracts.IDnsCache<byte[]> cache = new Dns.DnsCache<byte[]>();
            var invalidKeyResult = cache.Get("invalidTestKey");
            Xunit.Assert.Null(invalidKeyResult);
        }

        [Fact]
        public void Test2() {
            Dns.Contracts.IDnsCache<byte[]> cache = new Dns.DnsCache<byte[]>();

            string key = "sampleCacheKey";
            byte[] data = Encoding.ASCII.GetBytes("test");
            Int32 ttl = 10;

            cache.Set(key, data, ttl);
            var result = cache.Get(key);

            Xunit.Assert.True(data.SequenceEqual(result));

            var invalidKeyResult = cache.Get("invalidTestKey");
            Xunit.Assert.Null(invalidKeyResult);
        }
    }
}