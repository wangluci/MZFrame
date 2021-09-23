using Common.Redis;
using Microsoft.Extensions.Options;
using System;
namespace AuthService
{
    public class AuthRedisHelper : RedisHelper
    {
        public AuthRedisHelper(IOptions<AuthOption> conf) : base(conf.Value.redisconn, 0) { }
    }
}
