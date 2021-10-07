using System;

namespace AuthService
{
    public class Input_Role
    {
        public long key { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string[] permissions { get; set; }
    }
}
