using System;
using System.Data;
using System.Text;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using MyAccess.Aop;

namespace MyAccess.DB
{
    public abstract class DbHelp : IDisposable, IDbHelp
    {
        protected DbConnection mConn;
        protected DbCommand mCommand;
        protected string mConnString;
        protected DbTransaction mDbTrans;
        /// <summary>
        /// 是否禁止清参数
        /// </summary>
        protected bool mDiableClearParam;
        public bool IsNoTran()
        {
            return Equals(mDbTrans, null);
        }

        /// <summary>
        /// 当前参数集
        /// </summary>
        protected List<DbParameter> mDbParamters;
        public List<DbParameter> DBParamters { get { return mDbParamters; } }
        ~DbHelp()
        {
            Dispose(false);
        }
        private void Dispose(bool disposing)
        {
            Close();
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void DiableClearParam()
        {
            mDiableClearParam = true;
        }
        public void EnableAndClearParam()
        {
            mDiableClearParam = false;
            ClearParams();
        }
        public void Dispose()
        {
            Dispose(true);

        }
        public DbHelp(string connectionStr)
        {
            mDiableClearParam = false;
            mConnString = connectionStr;
            mDbParamters = new List<DbParameter>();
        }
        public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateCommand();
        public abstract DbDataAdapter CreateDataAdapter();



        public string ConnectionString
        {
            get { return mConnString; }
        }
        public DbTransaction Transaction
        {
            get { return mDbTrans; }
        }
        public DbConnection Connection
        {
            get { return mConn; }
        }
        public DbCommand Command
        {
            get { return mCommand; }
        }

        public void BeginTran()
        {
            BeginTran(Isolation.DEFAULT);
        }
        public void BeginTran(Isolation level)
        {
            if (Equals(mDbTrans, null))
            {
                Open();
                switch (level)
                {
                    case Isolation.DEFAULT:
                        mDbTrans = mConn.BeginTransaction();
                        break;
                    case Isolation.READ_UNCOMITTED:
                        mDbTrans = mConn.BeginTransaction(IsolationLevel.ReadUncommitted);
                        break;
                    case Isolation.READ_COMMITED:
                        mDbTrans = mConn.BeginTransaction(IsolationLevel.ReadCommitted);
                        break;
                    case Isolation.REPEATABLE_READ:
                        mDbTrans = mConn.BeginTransaction(IsolationLevel.RepeatableRead);
                        break;
                    case Isolation.SERIALIZABLE:
                        mDbTrans = mConn.BeginTransaction(IsolationLevel.Serializable);
                        break;
                }
                mCommand.Transaction = mDbTrans;
            }
        }
        public void Commit()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    mDbTrans.Commit();
                }
            }
            finally
            {
                Close();
            }
        }
        public void RollBack()
        {
            try
            {
                if (!Equals(mDbTrans, null))
                {
                    mDbTrans.Rollback();
                }
            }
            finally
            {
                Close();
            }
        }
        public void Open()
        {
            mConn = CreateConnection();
            mConn.ConnectionString = mConnString;
            mCommand = CreateCommand();
            mCommand.Connection = mConn;
            if (mConn.State == ConnectionState.Closed)
            {
                mConn.Open();
            }
        }

        /// <summary>
        /// 执行Sql操作
        /// </summary>
        /// <param name="docommand"></param>
        public void DoCommand(IDoCommand docommand)
        {
            if (!Equals(mDbTrans, null))
            {
                docommand.Excute(this);
            }
            else
            {
                Open();
                try
                {
                    docommand.Excute(this);
                }
                finally
                {
                    Close();
                }
            }
        }

        public void InitParams()
        {
            foreach (DbParameter p in mDbParamters)
            {
                mCommand.Parameters.Add(p);
            }
        }


        public void ClearParams()
        {
            if (!mDiableClearParam)
            {
                mDbParamters.Clear();
            }
            if (!Equals(mCommand, null))
            {
                mCommand.Parameters.Clear();
            }
        }

        public void Close()
        {
            if (!Equals(mDbTrans, null))
            {
                try
                {
                    mDbTrans.Dispose();
                }
                finally
                {
                    mDbTrans = mCommand.Transaction = null;
                }
            }

            if (!Equals(mCommand, null))
            {
                try
                {
                    mCommand.Dispose();
                }
                finally
                {
                    mCommand = null;
                }
            }

            if (!Equals(mConn, null) && mConn.State == ConnectionState.Open)
            {
                try
                {
                    mConn.Close();
                }
                finally
                {
                    mConn = null;
                }
            }
        }



        public void CopyDbParamFrom(DbParameter[] parameters)
        {
            if (!Equals(parameters, null))
            {
                ClearParams();
                mDbParamters.AddRange(parameters);
            }
        }

        /// <summary>
        /// 通过对象设置sql语句的输入参数生成的参数名字以@开头
        /// </summary>
        /// <param name="obj"></param>
        public void CopyParamFrom(object obj)
        {
            Type curObjType = obj.GetType();
            PropertyInfo[] myProInfos = curObjType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                AutoDbParam(pi.PropertyType, "@" + pi.Name, pi.GetValue(obj, null), ParameterDirection.Input);
            }
        }
        /// <summary>
        /// 会先判断是否为IBaseEntity实体，再拷贝参数
        /// </summary>
        /// <param name="obj"></param>
        public void AddParamFrom(object obj)
        {
            IBaseEntity be = obj as IBaseEntity;
            if (be == null)
            {
                CopyParamFrom(obj);
            }
            else
            {
                PropertyInfo[] myProInfos = be.GetUsedPropertys();
                for (int i = 0; i < myProInfos.Length; i++)
                {
                    PropertyInfo pi = myProInfos[i];
                    AddParam("@" + pi.Name, pi.GetValue(obj, null));
                }
            }
        }
        public void AddParam(string param, object value)
        {
            if (value == null)
            {
                AutoDbParam(typeof(string), param, DBNull.Value, ParameterDirection.Input);
            }
            else
            {
                AutoDbParam(value.GetType(), param, value, ParameterDirection.Input);
            }
        }
        public void AddReturnParam(Type tp, string param)
        {
            AutoDbParam(tp, param, DBNull.Value, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// C#类型转数据库类型
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        protected abstract void AutoDbParam(Type tp, string name, object val, ParameterDirection direct);
    }
}
