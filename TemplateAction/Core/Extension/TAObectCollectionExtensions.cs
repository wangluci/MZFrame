using System;
using System.Reflection;
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
        public static bool TryGet(this ITAObjectCollection collection, string key, Type t,out object result)
        {
            if (collection.TryGet(key, out result))
            {
                if (TAConverter.Instance.TryConvert(result, t, out result))
                {
                    return true;
                }
            }
            return false;
        }
        public static bool OldMapping(this ITAObjectCollection collection, ParameterInfo pi, out object result)
        {
            Type t = pi.ParameterType;
            string key = pi.Name;
            if (t.IsClass && t != typeof(string))
            {
                if (t.IsArray)
                {
                    string tlists = collection.Cast<string>(key + "[]", null);
                    if (tlists == null)
                    {
                        tlists = collection.Cast<string>(key, null);
                    }

                    string[] sarr;
                    if (tlists != null)
                    {
                        sarr = tlists.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    }
                    else
                    {
                        sarr = new string[0];
                    }

                    if (t == typeof(int[]))
                    {
                        int[] arr = new int[sarr.Length];
                        for (int i = 0; i < sarr.Length; i++)
                        {
                            string s = sarr[i];
                            int ti = 0;
                            if (int.TryParse(s, out ti))
                            {
                                arr[i] = ti;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        result = arr;
                        return true;
                    }
                    else if (t == typeof(long[]))
                    {
                        long[] arr = new long[sarr.Length];
                        for (int i = 0; i < sarr.Length; i++)
                        {
                            string s = sarr[i];
                            long ti = 0;
                            if (long.TryParse(s, out ti))
                            {
                                arr[i] = ti;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        result = arr;
                        return true;
                    }
                    else if (t == typeof(decimal[]))
                    {
                        decimal[] arr = new decimal[sarr.Length];
                        for (int i = 0; i < sarr.Length; i++)
                        {
                            string s = sarr[i];
                            decimal ti = 0;
                            if (decimal.TryParse(s, out ti))
                            {
                                arr[i] = ti;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        result = arr;
                        return true;
                    }
                    else
                    {
                        result = sarr;
                        return true;
                    }
                }
                else
                {
                    object ttobj = Activator.CreateInstance(t);
                    if (ttobj == null)
                    {
                        result = null;
                        return false;
                    }

                    PropertyInfo[] myallProinfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                    foreach (PropertyInfo myProInfo in myallProinfos)
                    {
                        if (myProInfo.GetIndexParameters().Length != 0) continue;
                        object tvalobj;
                        if (collection.TryGet(myProInfo.Name, myProInfo.PropertyType, out tvalobj))
                        {
                            myProInfo.SetValue(ttobj, tvalobj, null);
                        }
                    }
                    result = ttobj;
                    return true;
                }
            }
            else
            {
                if (collection.TryGet(key, t, out result))
                {
                    return true;
                }
                else
                {
                    if (pi.DefaultValue != DBNull.Value)
                    {
                        result = pi.DefaultValue;
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
