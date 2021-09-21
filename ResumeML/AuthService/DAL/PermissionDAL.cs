using Common.MySql;
using MyAccess.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class PermissionDAL : MySqlSupport
    {
        public PermissionDAL(string connstr) : base(connstr) { }
        public virtual List<UserPermission> GetUserPermissionByCode(long uid, string code)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, code);
            DoQuerySql<UserPermission> execsql = new DoQuerySql<UserPermission>("select * from mz_user_permission where UserId=@UserId and RightCode=@RightCode");
            help.DoCommand(execsql);
            return execsql.ToList();
        }
        public virtual bool HasRolePermissionByCode(long uid, string code)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, code);
            DoQuerySql<long> execsql = new DoQuerySql<long>("select nrr.RoleRightID from mz_role_permission nrr left join mz_user_role nur on nrr.RoleID = nur.RoleID where UserId=@UserId and RightCode=@RightCode");
            help.DoCommand(execsql);
            return execsql.Count() > 0;
        }

        public virtual bool HasUserPermission(long uid, string module)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, MyAccess.Filter.HtmlFilter.SqlLikeFilter(module) + "/%");
            DoQuerySql<long> execsql = new DoQuerySql<long>("select UserRightID from mz_user_permission where UserId=@UserId and RightType=0 and RightCode like @RightCode");
            help.DoCommand(execsql);
            return execsql.Count() > 0;
        }
        public virtual bool HasRolePermission(long uid, string module)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            string xx = MyAccess.Filter.HtmlFilter.SqlLikeFilter(module);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, MyAccess.Filter.HtmlFilter.SqlLikeFilter(module) + "/%");
            DoQuerySql<long> execsql = new DoQuerySql<long>("select nrr.RoleRightID from mz_role_permission nrr left join mz_user_role nur on nrr.RoleID = nur.RoleID where UserId=@UserId and RightCode like @RightCode");
            help.DoCommand(execsql);
            return execsql.Count() > 0;
        }
    }
}
