using System;
using System.Collections.Generic;
using System.Reflection;

namespace TemplateAction.Core
{
    public class ArraryMapping : IParamMapping
    {
        public object Mapping(ITAObjectCollection param, ParameterInfo pi)
        {
            string tlists = param.Cast<string>(pi.Name + "[]", null);
            if (string.IsNullOrEmpty(tlists))
            {
                tlists = param.Cast<string>(pi.Name, null);
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

            if (pi.ParameterType == typeof(int[]))
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
                return arr;
            }
            else if (pi.ParameterType == typeof(long[]))
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
                return arr;
            }
            else if (pi.ParameterType == typeof(decimal[]))
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
                return arr;
            }
            else
            {
                return sarr;
            }
        }
    }
}
