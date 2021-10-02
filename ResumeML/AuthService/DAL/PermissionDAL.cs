using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class PermissionDAL : MySqlSupport
    {
        public PermissionDAL(IOptions<AuthOption> conf) : base(conf.Value.connstr) { }
        public virtual List<MZ_UserPermission> GetUserPermissionByCode(long uid, string code)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, code);
            DoQuerySql<MZ_UserPermission> execsql = new DoQuerySql<MZ_UserPermission>("select * from mz_user_permission where UserId=@UserId and RightCode=@RightCode");
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
        /// <summary>
        /// 获取用户所有角色权限
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public virtual List<string> GetRolePermissionsByUser(long uid)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            DoQuerySql<string> execsql = new DoQuerySql<string>("select distinct nrr.RightCode from mz_role_permission nrr left join mz_user_role nur on nrr.RoleID = nur.RoleID where UserId=@UserId");
            help.DoCommand(execsql);
            return execsql.ToList();
        }
        public virtual List<MZ_UserPermission> GetUserPermissions(long uid)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            DoQuerySql<MZ_UserPermission> execsql = new DoQuerySql<MZ_UserPermission>("select * from mz_user_permission where UserId=@UserId");
            help.DoCommand(execsql);
            return execsql.ToList();
        }
        public virtual List<string> GetRolePermissions(long role)
        {
            help.AddInParam("@RoleID", MySqlDbType.Int64, role);
            DoQuerySql<string> execsql = new DoQuerySql<string>("select RightCode from mz_role_permission where RoleID=@RoleID");
            help.DoCommand(execsql);
            return execsql.ToList();
        }
        public virtual bool IsRoot(long uid)
        {
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            DoQuerySql<long> dqc = new DoQuerySql<long>("select ur.RoleID from mz_user_role ur inner join mz_role r on ur.RoleID=r.RoleID where UserId=@UserId and r.RoleType=1");
            help.DoCommand(dqc);
            return dqc.Count() > 0;
        }
    }
}
