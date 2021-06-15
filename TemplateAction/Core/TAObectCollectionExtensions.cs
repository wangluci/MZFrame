using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateAction.Common;

namespace TemplateAction.Core
{
    public static class TAObectCollectionExtensions
    {
        public static T Cast<T>(this ITAObjectCollection collection, string key, T def)
        {
            object result;
            if (collection.TryGet(key, out result))
            {
                return TAConverter.Cast(result, def);
            }
            else
            {
                return def;
            }
        }

        public static bool TryConvert(this ITAObjectCollection collection, string key, Type targetType, out object result)
        {
            if (collection.TryGet(key, out result))
            {
                if (TAConverter.Instance.TryConvert(result, targetType, out result))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
