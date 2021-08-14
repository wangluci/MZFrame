using MyAccess.Aop.DAL;
using MyAccess.DB;
using System.Threading;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    /// <summary>
    /// 异步DBHelp事务管理器
    /// </summary>
    public class DBTransMan
    {
        private volatile static DBTransMan mInstance = null;
        private static readonly object lockHelper = new object();

        private static AsyncLocal<DBStoreItem> _storeItem = new AsyncLocal<DBStoreItem>();

        private DBTransMan()
        {
        }
        public static DBTransMan Instance()
        {
            if (mInstance == null)
            {
                lock (lockHelper)
                {
                    if (mInstance == null)
                        mInstance = new DBTransMan();
                }
            }
            return mInstance;
        }
        public bool IsOpenTrans()
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
        public void BeginTrans()
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
