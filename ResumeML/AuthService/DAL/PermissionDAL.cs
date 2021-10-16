using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.Aop;
using MyAccess.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class PermissionDAL : MySqlSupport
    {
        public PermissionDAL(IOptions<AuthOption> conf) : base(conf.Value.connstr) { }
        public virtual bool ExistPermission(long uid, string module, string action)
        {
            string code = string.Format("{0}/{1}", module, action);
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@RightCode", MySqlDbType.VarChar, code);
            DoQuerySql<MZ_UserPermission> execsql = new DoQuerySql<MZ_UserPermission>("select * from mz_user_permission where UserId=@UserId and RightCode=@RightCode");
            help.DoCommand(execsql);
            List<MZ_UserPermission> nurlist = execsql.ToList();
            if (nurlist.Count > 0)
            {
                bool hasright = true;
                foreach (MZ_UserPermission nur in nurlist)
                {
                    if (nur.RightType == 1)
                    {
                        hasright = false;
                        break;
                    }
                }
                return hasright;
            }
            else
            {
                help.AddInParam("@UserId", MySqlDbType.Int64, uid);
                help.AddInParam("@RightCode", MySqlDbType.VarChar, code);
                DoQuerySql<long> execsql2 = new DoQuerySql<long>("select nrr.RoleRightID from mz_role_permission nrr left join mz_user_role nur on nrr.RoleID = nur.RoleID where UserId=@UserId and RightCode=@RightCode");
                help.DoCommand(execsql2);
                return execsql2.Count > 0;
            }
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
            return dqc.Count > 0;
        }
        [Trans]
        public virtual void SetRolePermissions(long id, MZ_Role_Permission[] permissions)
        {
            DoDelete dd = new DoDelete("mz_role_permission", "RoleID=" + id);
            help.DoCommand(dd);
            if (permissions.Length > 0)
            {
                DoInsert<MZ_Role_Permission> isert = new DoInsert<MZ_Role_Permission>(permissions, "mz_role_permission");
                help.DoCommand(isert);
            }
        }
    }
}
