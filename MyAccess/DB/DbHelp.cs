﻿using System;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Collections.Generic;
using MyAccess.Aop;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public abstract class DbHelp : IDisposable, IDbHelp
    {
        protected DbConnection mConn;
        protected string mConnString;
        protected DbTransaction mDbTrans;
        public DbTransaction DbTrans
        {
            get { return mDbTrans; }
        }
        /// <summary>
        /// 是否禁止清参数
        /// </summary>
        protected bool mDiableClearParam;

        /// <summary>
        /// 当前参数集
        /// </summary>
        protected List<DbParameter> mDbParamters;
        public List<DbParameter> DbParamters { get { return mDbParamters; } }
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

        private void InitBeginTran(Isolation level)
        {
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
        }
        public void BeginTran(Isolation level = Isolation.DEFAULT)
        {
            if (Equals(mDbTrans, null))
            {
                Open();
                InitBeginTran(level);
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
            if (mConn.State == ConnectionState.Closed)
            {
                mConn.Open();
            }
        }
        /// <summary>
        /// 批量执行Command
        /// </summary>
        /// <param name="docommands"></param>
        public void DoCommandBatch(params IDoCommand[] docommands)
        {
            if (!Equals(mDbTrans, null))
            {
                foreach (IDoCommand dc in docommands)
                {
                    dc.Excute(this);
                }
            }
            else
            {
                Open();
                try
                {
                    foreach (IDoCommand dc in docommands)
                    {
                        dc.Excute(this);
                    }
                }
                finally
                {
                    Close();
                }
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



        public void ClearParams()
        {
            if (!mDiableClearParam)
            {
                mDbParamters.Clear();
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
                    mDbTrans = null;
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


        #region 异步操作
        public async Task BeginTranAsync(Isolation level = Isolation.DEFAULT)
        {
            if (Equals(mDbTrans, null))
            {
                await OpenAsync();
                InitBeginTran(level);
            }
        }
        public async Task OpenAsync()
        {
            mConn = CreateConnection();
            mConn.ConnectionString = mConnString;
            if (mConn.State == ConnectionState.Closed)
            {
                await mConn.OpenAsync();
            }
        }

        public async Task DoCommandAsync(IDoCommand docommand)
        {
            if (!Equals(mDbTrans, null))
            {
                await docommand.ExcuteAsync(this);
            }
            else
            {
                await OpenAsync();
                try
                {
                    await docommand.ExcuteAsync(this);
                }
                finally
                {
                    Close();
                }
            }
        }
        public async Task DoCommandBatchAsync(params IDoCommand[] docommands)
        {
            if (!Equals(mDbTrans, null))
            {
                foreach (IDoCommand dc in docommands)
                {
                    await dc.ExcuteAsync(this);
                }
            }
            else
            {
                await OpenAsync();
                try
                {
                    foreach (IDoCommand dc in docommands)
                    {
                        await dc.ExcuteAsync(this);
                    }
                }
                finally
                {
                    Close();
                }
            }
        }
        #endregion

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
        public void AddParamFrom(object obj)
        {
            Type curObjType = obj.GetType();
            PropertyInfo[] myProInfos = curObjType.GetProperties();
            for (int i = 0; i < myProInfos.Length; i++)
            {
                PropertyInfo pi = myProInfos[i];
                AutoDbParam(pi.Name, pi.GetValue(obj, null), ParameterDirection.Input);
            }
        }
        /// <summary>
        /// 会先判断是否为IBaseEntity实体，再拷贝参数
        /// </summary>
        /// <param name="obj"></param>
        public void AddParamFromEntity(object obj)
        {
            IBaseEntity be = obj as IBaseEntity;
            if (be == null)
            {
                AddParamFrom(obj);
            }
            else
            {
                PropertyInfo[] myProInfos = be.GetUsedPropertys();
                for (int i = 0; i < myProInfos.Length; i++)
                {
                    PropertyInfo pi = myProInfos[i];
                    AddParam(pi.Name, pi.GetValue(obj, null));
                }
            }
        }
        public void AddParam(string param, object value)
        {
            AutoDbParam(param, value, ParameterDirection.Input);
        }
        public string AddParamAndReturn(string param, object value)
        {
            AddParam(param, value);
            return NameToDbParam(param);
        }
        /// <summary>
        /// C#类型转数据库类型
        /// </summary>
        /// <param name="tp"></param>
        /// <returns></returns>
        protected abstract void AutoDbParam(string name, object val, ParameterDirection direct);
        /// <summary>
        /// 参数名转为各个数据库所需参数格式名称
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected abstract string NameToDbParam(string param);

        public abstract DbConnection CreateConnection();
        public abstract DbCommand CreateCommand();
        public abstract DbDataAdapter CreateDataAdapter();

    }
}
