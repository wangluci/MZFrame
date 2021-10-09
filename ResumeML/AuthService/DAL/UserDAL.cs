using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.Aop;
using MyAccess.DB;
using MySqlConnector;
using System;
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
   
        private void _SetRolePermissions(long id, MZ_Role_Permission[] permissions)
        {
            DoDelete dd = new DoDelete("mz_role_permission", "RoleID=" + id);
            help.DoCommand(dd);
            if (permissions.Length > 0)
            {
                DoInsert<MZ_Role_Permission> isert = new DoInsert<MZ_Role_Permission>(permissions, "mz_role_permission");
                help.DoCommand(isert);
            }
        }
        [Trans]
        public virtual long AddRole(MZ_Role role)
        {
            DoInsertOfMySql<MZ_Role> di = new DoInsertOfMySql<MZ_Role>(role, "mz_role");
            help.DoCommand(di);
            long roleid = di.LastInsertedId;
            MZ_Role_Permission[] role_permiss = new MZ_Role_Permission[role.permissions.Length];
            for (int i = 0; i < role_permiss.Length; i++)
            {
                role_permiss[i] = new MZ_Role_Permission();
                role_permiss[i].RoleID = roleid;
                role_permiss[i].RightCode = role.permissions[i];
            }
            _SetRolePermissions(roleid, role_permiss);
            return roleid;
        }
        [Trans]
        public virtual void UpdateRole(MZ_Role role)
        {
            DoUpdate du = new DoUpdate(role, "mz_role");
            help.DoCommand(du);
            if (role.permissions != null)
            {
                MZ_Role_Permission[] role_permiss = new MZ_Role_Permission[role.permissions.Length];
                for (int i = 0; i < role_permiss.Length; i++)
                {
                    role_permiss[i] = new MZ_Role_Permission();
                    role_permiss[i].RoleID = role.RoleID;
                    role_permiss[i].RightCode = role.permissions[i];
                }
                _SetRolePermissions(role.RoleID, role_permiss);
            }
        }
        [Trans]
        public virtual void DeleteRole(long id)
        {
            help.AddInParam("@RoleID", MySqlDbType.Int64, id);
            DoDelete dd = new DoDelete("mz_role", "RoleID=@RoleID");
            help.DoCommand(dd);
            _SetRolePermissions(id, Array.Empty<MZ_Role_Permission>());
        }
    }
}
