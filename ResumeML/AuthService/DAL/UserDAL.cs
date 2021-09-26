using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySqlConnector;

namespace AuthService
{
    public class UserDAL : MySqlSupport
    {
        public UserDAL(IOptions<AuthOption> conf) : base(conf.Value.connstr) { }

        public virtual AdminInfo GetAdminByName(string username)
        {
            help.AddInParam("@UserName", MySqlDbType.VarChar, username);
            DoQuerySql<AdminInfo> execsql = new DoQuerySql<AdminInfo>("select * from mz_admin where UserName=@UserName");
            help.DoCommand(execsql);
            return execsql.ToFirst();
        }
        public virtual AdminInfo GetAdminById(long uid)
        {
            help.AddInParam("@Id", MySqlDbType.Int64, uid);
            DoQuerySql<AdminInfo> execsql = new DoQuerySql<AdminInfo>("select * from mz_admin where Id=@Id");
            help.DoCommand(execsql);
            return execsql.ToFirst();
        }
    }
}
