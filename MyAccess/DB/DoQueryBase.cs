using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace MyAccess.DB
{
    public abstract class DoQueryBase : IDoSqlCommand
    {
        protected string mSql;
        protected DataSet mData;

        public DataRowCollection Rows
        {
            get { return mData.Tables[0].Rows; }
        }
        /// <summary>
        /// 存在返回第一个Table否则返回null
        /// </summary>
        public DataTable Table
        {
            get
            {
                if (mData.Tables.Count == 0) return null;
                return mData.Tables[0];
            }
        }
        public int Count(int i = 0)
        {
            if (mData.Tables.Count <= i)
            {
                return 0;
            }
            return mData.Tables[i].Rows.Count;
        }
        public DataSet Data
        {
            get { return mData; }
        }
        public DoQueryBase()
        {
            mData = new DataSet();
        }
        public void SetSql(string sql)
        {
            mSql = sql;
        }
        /// <summary>
        /// 获取返回模型列表的第一个，没有则返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ToFirst<T>()
        {
            List<T> tmplist = ToList<T>();
            if (tmplist.Count <= 0) return default(T);
            return tmplist[0];
        }
        public T ToFirstOrDefault<T>(T def)
        {
            List<T> tmplist = ToList<T>();
            if (tmplist.Count <= 0) return def;
            return tmplist[0];
        }
        public T ToFirstOrDefault<T>(int i, T def)
        {
            List<T> tmplist = ToList<T>(i);
            if (tmplist.Count <= 0) return def;
            return tmplist[0];
        }
        public T ToFirst<T>(int i)
        {
            List<T> tmplist = ToList<T>(i);
            if (tmplist.Count <= 0) return default(T);
            return tmplist[0];
        }
        public T[] ToArray<T>()
        {
            if (mData.Tables.Count <= 0)
            {
                return new T[0];
            }
            return ToArray<T>(mData.Tables[0]);
        }
        public T[] ToArray<T>(int i)
        {
            if (mData.Tables.Count <= i) return new T[0];
            return ToArray<T>(mData.Tables[i]);
        }
        public T[] ToArray<T>(DataTable dt)
        {
            T[] newList = new T[dt.Rows.Count];
            Assigned<T>(dt, (t, i) =>
            {
                newList[i] = t;
            });
            return newList;
        }
        public List<T> ToList<T>()
        {
            if (mData.Tables.Count <= 0)
            {
                return new List<T>();
            }
            return ToList<T>(mData.Tables[0]);
        }
        public List<T> ToList<T>(int i)
        {
            if (mData.Tables.Count <= i) return new List<T>();
            return ToList<T>(mData.Tables[i]);
        }
        /// <summary>
        /// 返回模型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> newList = new List<T>(dt.Rows.Count);
            Assigned<T>(dt, (t, i) =>
            {
                newList.Add(t);
            });
            return newList;
        }

        /// <summary>
        /// 真正的赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <param name="fun"></param>
        public static void Assigned<T>(DataTable dt, Action<T, int> fun)
        {
            Type temptype = typeof(T);
            if (temptype.IsInterface)
            {
                throw new Exception("数据库转实体不能为接口");
            }
            bool isvaltmp = !temptype.IsClass || temptype == typeof(string);
            for (int index = 0; index < dt.Rows.Count; index++)
            {
                DataRow dr = dt.Rows[index];
                if (isvaltmp)
                {
                    if (dr.Table.Columns.Count > 0)
                    {
                        if (temptype == typeof(string))
                        {
                            fun((T)(object)Core.TypeConvert.ToString(dr[0]), index);
                        }
                        else if (temptype == typeof(Int32))
                        {
                            fun((T)(object)Core.TypeConvert.ToInt(dr[0]), index);
                        }
                        else if (temptype == typeof(Int64))
                        {
                            fun((T)(object)Core.TypeConvert.ToLong(dr[0]), index);
                        }
                        else if (temptype == typeof(double))
                        {
                            fun((T)(object)Core.TypeConvert.CDouble(dr[0]), index);
                        }
                        else if (temptype == typeof(decimal))
                        {
                            fun((T)(object)Core.TypeConvert.CDecimal(dr[0]), index);
                        }
                        else if (temptype.IsEnum)
                        {
                            fun((T)(object)Core.TypeConvert.ToInt(dr[0]), index);
                        }
                        else if (temptype == typeof(bool))
                        {
                            fun((T)(object)Core.TypeConvert.StrToBool(dr[0]), index);
                        }
                        else if (temptype == typeof(DateTime))
                        {
                            fun((T)(object)Core.TypeConvert.CDate(dr[0]), index);
                        }
                    }
                }
                else
                {
                    T model = (T)Activator.CreateInstance(temptype);
                    Type targT = model.GetType();
                    for (int i = 0; i < dr.Table.Columns.Count; i++)
                    {
                        PropertyInfo propertyInfo = targT.GetProperty(dr.Table.Columns[i].ColumnName);
                        if (propertyInfo != null && dr[i] != DBNull.Value)
                        {
                            if (propertyInfo.PropertyType == typeof(DateTime?) || propertyInfo.PropertyType == typeof(DateTime))
                            {
                                propertyInfo.SetValue(model, MyAccess.Core.TypeConvert.CDate(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(decimal))
                            {
                                propertyInfo.SetValue(model, MyAccess.Core.TypeConvert.CDecimal(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(string))
                            {
                                propertyInfo.SetValue(model, MyAccess.Core.TypeConvert.ToString(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(Int32))
                            {
                                propertyInfo.SetValue(model, MyAccess.Core.TypeConvert.ToInt(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(Int64))
                            {
                                propertyInfo.SetValue(model, MyAccess.Core.TypeConvert.ToLong(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType.IsEnum)
                            {
                                propertyInfo.SetValue(model, Core.TypeConvert.ToInt(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(bool))
                            {
                                propertyInfo.SetValue(model, Core.TypeConvert.StrToBool(dr[i]), null);
                            }
                            else if (propertyInfo.PropertyType == typeof(byte))
                            {
                                propertyInfo.SetValue(model, Convert.ToByte(dr[i]), null);
                            }
                            else
                            {
                                propertyInfo.SetValue(model, dr[i], null);
                            }

                        }
                    }
                    fun(model, index);
                }

            }
        }

        public abstract void Excute(DbHelp help);

        public string GetSql()
        {
            return mSql;
        }
    }
}
