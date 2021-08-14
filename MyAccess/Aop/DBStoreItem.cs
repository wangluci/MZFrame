using MyAccess.DB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyAccess.Aop
{
    internal class DBStoreItem
    {
        protected List<IDbHelp> mHelpStoreList = new List<IDbHelp>();
        public void OpenDB(IDbHelp dbHelp, Isolation level = Isolation.DEFAULT)
        {
            mHelpStoreList.Add(dbHelp);
            dbHelp.BeginTran(level);
        }
        public async Task OpenDBAsync(IDbHelp dbHelp, Isolation level = Isolation.DEFAULT)
        {
            mHelpStoreList.Add(dbHelp);
            await dbHelp.BeginTranAsync(level);
        }
        public void Commit()
        {
            foreach (IDbHelp item in mHelpStoreList)
            {
                item.Commit();
            }
            mHelpStoreList.Clear();
        }
        public void RollBack()
        {
            foreach (IDbHelp item in mHelpStoreList)
            {
                item.RollBack();
            }
            mHelpStoreList.Clear();
        }
    }
}
