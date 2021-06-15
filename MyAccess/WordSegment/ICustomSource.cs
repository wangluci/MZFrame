using System;
using System.Collections.Generic;

namespace MyAccess.WordSegment
{
    public  interface ICustomSource
    {
        List<WordAttribute> DictFrom();
        bool DictFromChanged();
    }
}
