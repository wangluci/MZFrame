﻿using MyAccess.DB.Attr;
using System;

namespace TestService.Model
{
    public class testtb
    {
        [ID]
        public virtual int testid { get; set; }
        public virtual string testdes { get; set; }
    }
}