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

        public virtual List<MZ_AdminInfo> GetAllUser(string key, int page, int pagesize, out int total)
        {
            total = 0;
            string sqlwhere = string.Empty;
            if (!string.IsNullOrEmpty(key))
            {
                help.AddInParam("@Key", MySqlDbType.VarChar, key + "%");
                sqlwhere += " and (RealName like @Key or UserName like @Key)";
            }

            int start_row = (page - 1) * pagesize;
            if (start_row < 0) start_row = 0;
            DoQuerySql<MZ_AdminInfo> querysql = new DoQuerySql<MZ_AdminInfo>("select SQL_CALC_FOUND_ROWS distinct * from mz_admin " + sqlwhere + " limit " + start_row + "," + pagesize);
            DoQuerySql<int> querytotal = new DoQuerySql<int>("select FOUND_ROWS()");
            help.DoCommand(new DoQueryGroup(querysql, querytotal));
            total = querytotal.ToFirst();
            return querysql.ToList();
        }
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


        [Trans]
        public virtual long AddRole(MZ_Role role)
        {
            DoInsertOfMySql<MZ_Role> di = new DoInsertOfMySql<MZ_Role>(role, "mz_role");
            help.DoCommand(di);
            return di.LastInsertedId;
        }
        [Trans]
        public virtual void UpdateRole(MZ_Role role)
        {
            DoUpdate du = new DoUpdate(role, "mz_role");
            help.DoCommand(du);
        }
        [Trans]
        public virtual void DeleteRole(long id)
        {
            help.AddInParam("@RoleID", MySqlDbType.Int64, id);
            DoDelete dd = new DoDelete("mz_role", "RoleID=@RoleID");
            help.DoCommand(dd);
        }
    }
}
