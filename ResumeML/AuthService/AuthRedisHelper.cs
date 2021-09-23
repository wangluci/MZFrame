using Common.Redis;
using System;
namespace AuthService
{
    public class AuthRedisHelper : RedisHelper
    {
        public AuthRedisHelper(AuthOption conf) : base(conf.connstr, 0) { }
    }
}
