using System;
using Microsoft.Extensions.Options;

namespace AuthService
{
    public class AuthOption : IOptions<AuthOption>
    {
        public string connstr { get; set; }
        public string redisconn { get; set; }
        public AuthOption Value => this;
    }
}
