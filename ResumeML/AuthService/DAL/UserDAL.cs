using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySqlConnector;

namespace AuthService
{
    public class UserDAL : MySqlSupport
    {
        public UserDAL(IOptions<AuthOption> conf) : base(conf.Value.connstr) { }

        public virtual MZ_AdminInfo GetAdminByName(string username)
        {
            help.AddInParam("@UserName", MySqlDbType.VarChar, username);
            DoQuerySql<MZ_AdminInfo> execsql = new DoQuerySql<MZ_AdminInfo>("select * from mz_admin where UserName=@UserName");
            help.DoCommand(execsql);
            return execsql.ToFirst();
        }
        public virtual MZ_AdminInfo GetAdminById(long uid)
        {
            help.AddInParam("@Id", MySqlDbType.Int64, uid);
            DoQuerySql<MZ_AdminInfo> execsql = new DoQuerySql<MZ_AdminInfo>("select * from mz_admin where Id=@Id");
            help.DoCommand(execsql);
            return execsql.ToFirst();
        }
    }
}
