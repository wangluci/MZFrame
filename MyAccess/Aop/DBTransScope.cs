using MyAccess.DB;
using System.Threading;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// DBHelp事务域管理
    /// </summary>
    public class DBTransScope
    {
        private volatile static DBTransScope mInstance = null;
        private static readonly object lockHelper = new object();

        private static AsyncLocal<DBStoreItem> _storeItem = new AsyncLocal<DBStoreItem>();

        private DBTransScope()
        {
        }
        public static DBTransScope Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                        mInstance = new DBTransScope();
                }
            }
            return mInstance;
        }
        /// <summary>
        /// 是否启用域
        /// </summary>
        /// <returns></returns>
        public bool IsBegan()
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi == null)
            {
                return false;
            }
            return true;
        }
        public void OpenDB(IDbHelp dbhelp, Isolation level = Isolation.DEFAULT)
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi == null)
            {
                dbhelp.BeginTran(level);
                return;
            }
            dbsi.OpenDB(dbhelp, level);
        }
        public async Task OpenDBAsync(IDbHelp dbHelp, Isolation level = Isolation.DEFAULT)
        {
            DBStoreItem dbsi = _storeItem.Value;
            if (dbsi == null)
            {
                await dbHelp.BeginTranAsync(level);
                return;
            }
            await dbsi.OpenDBAsync(dbHelp, level);
        }
        public void BeginScope()
        {
            _storeItem.Value = new DBStoreItem();
        }
        public void Commit()
        {
            _storeItem.Value?.Commit();
            _storeItem.Value = null;
        }
        public void RollBack()
        {
            _storeItem.Value?.RollBack();
            _storeItem.Value = null;
        }

    }
}
