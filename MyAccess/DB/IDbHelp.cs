using System;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public enum Isolation
    {
        //使用数据库本身使用的隔离级别 ORACLE（读已提交） MySQL（可重复读）
        DEFAULT,
        //读未提交（脏读）最低的隔离级别，一切皆有可能。
        READ_UNCOMITTED,
        //读已提交，ORACLE默认隔离级别，有幻读以及不可重复读风险。
        READ_COMMITED,
        //可重复读，解决不可重复读的隔离级别，但还是有幻读风险
        REPEATABLE_READ,
        //串行化，最高隔离级别，杜绝一切隐患，缺点是效率低。
        SERIALIZABLE
    }
    public interface IDbHelp
    {
        bool IsNoTran();
        void BeginTran(Isolation level = Isolation.DEFAULT);
        Task BeginTranAsync(Isolation level = Isolation.DEFAULT);
        void Commit();
        void RollBack();
        void EnableAndClearParam();
    }
}
