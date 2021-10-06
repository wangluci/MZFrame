using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class DefatulParamMapping : IParamMapping
    {
        internal static bool TAObjectMapping(ITAObjectCollection collection, string key, Type t, out object result)
        {
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
                    result = null;
                    PropertyInfo[] myallProinfos = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                    foreach (PropertyInfo myProInfo in myallProinfos)
                    {
                        if (myProInfo.GetIndexParameters().Length != 0) continue;
                        object tvalobj;
                        if (collection.TryGet(myProInfo.Name, myProInfo.PropertyType, out tvalobj))
                        {
                            if (result == null)
                            {
                                result = Activator.CreateInstance(t);
                            }
                            myProInfo.SetValue(result, tvalobj, null);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                return collection.TryGet(key, t, out result);
            }
        }
        /// <summary>
        /// 默认参数绑定
        /// </summary>
        /// <param name="next"></param>
        /// <param name="ac"></param>
        /// <param name="pi"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Mapping(LinkedListNode<IParamMapping> next, TAAction ac, string key, Type t, out object result)
        {
            ITARequest req = ac.Context.Request;
            if (req.Query != null)
            {
                if (TAObjectMapping(req.Query, key, t, out result))
                {
                    return true;
                }
            }
            if (req.Form != null)
            {
                if (TAObjectMapping(req.Form, key, t, out result))
                {
                    return true;
                }
            }
            if (ac.ExtParams != null)
            {
                if (TAObjectMapping(ac.ExtParams, key, t, out result))
                {
                    return true;
                }
            }

            if (next == null)
            {
                result = null;
                return false;
            }
            return next.Value.Mapping(next.Next, ac, key, t, out result);
        }
    }
}
