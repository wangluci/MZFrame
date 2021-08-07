﻿using MyAccess.DB;
using System;
namespace MyAccess.Aop.DAL
{
    /// <summary>
    /// 同步用DAL层
    /// </summary>
    public abstract class DBSupport : IDBFactory
    {
        [ThreadStatic]
        protected static IDbHelp mDBHelp;
        protected string _connectionStr;
        public IDbHelp DBHelp
        {
            get { return mDBHelp; }
        }

        public string Key
        {
            get { return _connectionStr; }
        }

        public DBSupport(string connectionStr)
        {
            _connectionStr = connectionStr;
        }

        internal void InitHelp()
        {
            mDBHelp = DBMan.Instance().OpenDB(this);
        }

        public abstract IDbHelp CreateHelp();
    }
}
