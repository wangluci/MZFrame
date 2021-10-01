using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    /// <summary>
    /// 获取唯一返回值
    /// </summary>
    public class DoQueryScalar : IDoSqlCommand
    {
        private string mSql;
        private object mValue;
        public void SetSql(string sql)
        {
            mSql = sql;
        }

        public string GetSql()
        {
            return mSql;
        }

        public double GetValueDouble(double def)
        {
            double rt = def;
            if (mValue != null)
            {
                if (!double.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public decimal GetValueDecimal(decimal def)
        {
            decimal rt = def;
            if (mValue != null)
            {
                if (!decimal.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public long GetValueLong(long def)
        {
            long rt = def;
            if (mValue != null)
            {
                if (!long.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public int GetValueInt(int def)
        {
            int rt = def;
            if (mValue != null)
            {
                if (!int.TryParse(mValue.ToString(), out rt))
                {
                    rt = def;
                }
            }
            return rt;
        }
        public string GetValue(string def)
        {
            if (mValue != null)
            {
                return mValue.ToString();
            }
            return def;
        }
        public DoQueryScalar(string sql)
        {
            mSql = sql;
        }
        public void Excute(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = mSql;
            foreach (DbParameter p in help.DbParamters)
            {
                if (!command.Parameters.Contains(p.ParameterName))
                {
                    command.Parameters.Add(p);
                }
            }
            mValue = command.ExecuteScalar();
        }

  
        public async Task ExcuteAsync(DbHelp help)
        {
            DbCommand command = help.CreateCommand();
            command.Connection = help.Connection;
            if (help.DbTrans != null)
            {
                command.Transaction = help.DbTrans;
            }
            command.CommandType = CommandType.Text;
            command.CommandText = mSql;
            foreach (DbParameter p in help.DbParamters)
            {
                if (!command.Parameters.Contains(p.ParameterName))
                {
                    command.Parameters.Add(p);
                }
            }
            mValue = await command.ExecuteScalarAsync();
        }
    }
}
