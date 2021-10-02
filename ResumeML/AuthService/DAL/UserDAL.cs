using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySqlConnector;
using System.Collections.Generic;

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
        public virtual List<MZ_Role> GetAllRole()
        {
            DoQuerySql<MZ_Role> dqs = new DoQuerySql<MZ_Role>("select * from mz_role");
            help.DoCommand(dqs);
            return dqs.ToList();
        }
        public virtual MZ_Role GetRole(long id)
        {
            help.AddInParam("@RoleID", MySqlDbType.Int64, id);
            DoQuerySql<MZ_Role> dqs = new DoQuerySql<MZ_Role>("select * from mz_role where RoleID=@RoleID");
            help.DoCommand(dqs);
            return dqs.ToFirst();
        }
        public virtual long AddRole(MZ_Role role)
        {
            DoInsertOfMySql<MZ_Role> di = new DoInsertOfMySql<MZ_Role>(role, "mz_role");
            help.DoCommand(di);
            return di.LastInsertedId;
        }
        public virtual int UpdateRole(MZ_Role role)
        {
            DoUpdate du = new DoUpdate(role, "mz_role");
            help.DoCommand(du);
            return du.RowCount;
        }
    }
}
