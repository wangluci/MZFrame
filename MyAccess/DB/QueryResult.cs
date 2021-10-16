using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public class QueryResult<T>: IDoResult<DbDataReader>
    {
        private List<T> mList;
        public int Count
        {
            get
            {
                return mList.Count;
            }
        }
        /// <summary>
        /// 获取返回模型列表的第一个，没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToFirst()
        {
            if (mList.Count <= 0) return default(T);
            return mList[0];
        }
        public T ToFirstOrDefault(T def)
        {
            if (mList.Count <= 0) return def;
            return mList[0];
        }

        public T[] ToArray()
        {
            return mList.ToArray();
        }

        public List<T> ToList()
        {
            return mList;
        }
        public QueryResult()
        {
            mList = new List<T>();
        }
        private static IDictionary<Type, string> _mappings = new Dictionary<Type, string>()
        {
             {typeof(DateTime?), "DateTime?"},
             {typeof(DateTime), "DateTime"},
             {typeof(decimal), "decimal"},
             {typeof(string), "string"},
             {typeof(Int32), "Int32"},
             {typeof(Int64), "Int64"},
             {typeof(bool), "bool"},
             {typeof(byte), "byte"}
        };
        private T GetAssigned(DbDataReader dr)
        {
            Type temptype = typeof(T);
            if (temptype.IsInterface)
            {
                throw new Exception("数据库转实体不能为接口");
            }
            bool isvaltmp = !temptype.IsClass || temptype == typeof(string);
            if (isvaltmp)
            {
                return dr.GetFieldValue<T>(0);
            }
            else
            {
                T model = (T)Activator.CreateInstance(temptype);
                Type targT = model.GetType();

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = targT.GetProperty(dr.GetName(i));
                    if (propertyInfo == null)
                    {
                        continue;
                    }
                    if (dr.IsDBNull(i))
                    {
                        continue;
                    }
                    string mapval;
                    if (_mappings.TryGetValue(propertyInfo.PropertyType, out mapval))
                    {
                        switch (mapval)
                        {
                            case "DateTime?":
                                propertyInfo.SetValue(model, dr.GetFieldValue<DateTime?>(i), null);
                                break;
                            case "DateTime":
                                propertyInfo.SetValue(model, dr.GetFieldValue<DateTime>(i), null);
                                break;
                            case "decimal":
                                propertyInfo.SetValue(model, dr.GetFieldValue<decimal>(i), null);
                                break;
                            case "string":
                                propertyInfo.SetValue(model, dr.GetFieldValue<string>(i), null);
                                break;
                            case "Int32":
                                propertyInfo.SetValue(model, dr.GetFieldValue<Int32>(i), null);
                                break;
                            case "Int64":
                                propertyInfo.SetValue(model, dr.GetFieldValue<Int64>(i), null);
                                break;
                            case "bool":
                                propertyInfo.SetValue(model, dr.GetFieldValue<bool>(i), null);
                                break;
                            case "byte":
                                propertyInfo.SetValue(model, dr.GetFieldValue<byte>(i), null);
                                break;
                            default:
                                propertyInfo.SetValue(model, dr.GetFieldValue<object>(i), null);
                                break;
                        }
                    }
                    else
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(model, dr.GetFieldValue<Int32>(i), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(model, dr.GetFieldValue<object>(i), null);
                        }
                    }

                }
                return model;
            }
        }

        private async Task<T> GetAssignedAsync(DbDataReader dr)
        {
            Type temptype = typeof(T);
            if (temptype.IsInterface)
            {
                throw new Exception("数据库转实体不能为接口");
            }
            bool isvaltmp = !temptype.IsClass || temptype == typeof(string);
            if (isvaltmp)
            {
                return await dr.GetFieldValueAsync<T>(0);
            }
            else
            {
                T model = (T)Activator.CreateInstance(temptype);
                Type targT = model.GetType();

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    PropertyInfo propertyInfo = targT.GetProperty(dr.GetName(i));
                    if (propertyInfo == null)
                    {
                        continue;
                    }
                    if (await dr.IsDBNullAsync(i))
                    {
                        continue;
                    }
                    string mapval;
                    if (_mappings.TryGetValue(propertyInfo.PropertyType, out mapval))
                    {
                        switch (mapval)
                        {
                            case "DateTime?":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<DateTime?>(i), null);
                                break;
                            case "DateTime":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<DateTime>(i), null);
                                break;
                            case "decimal":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<decimal>(i), null);
                                break;
                            case "string":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<string>(i), null);
                                break;
                            case "Int32":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<Int32>(i), null);
                                break;
                            case "Int64":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<Int64>(i), null);
                                break;
                            case "bool":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<bool>(i), null);
                                break;
                            case "byte":
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<byte>(i), null);
                                break;
                            default:
                                propertyInfo.SetValue(model, await dr.GetFieldValueAsync<object>(i), null);
                                break;
                        }
                    }
                    else
                    {
                        if (propertyInfo.PropertyType.IsEnum)
                        {
                            propertyInfo.SetValue(model, await dr.GetFieldValueAsync<Int32>(i), null);
                        }
                        else
                        {
                            propertyInfo.SetValue(model, await dr.GetFieldValueAsync<object>(i), null);
                        }
                    }

                }
                return model;
            }
        }
        public void SetResult(DbDataReader result)
        {
            mList.Add(GetAssigned(result));
        }
        public async Task SetResultAsync(DbDataReader result)
        {
            mList.Add(await GetAssignedAsync(result));
        }
    }
}
