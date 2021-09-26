using Common.MySql;
using Microsoft.Extensions.Options;
using MyAccess.DB;
using MySqlConnector;
using System;
using System.Collections.Generic;

namespace AuthService
{
    public class AuthDAL : MySqlSupport
    {
        public AuthDAL(IOptions<AuthOption> conf) : base(conf.Value.connstr) { }


        public virtual int AddSysLog(SysLog log)
        {
            DoInsert<SysLog> di = new DoInsert<SysLog>(log, "mz_sys_log");
            help.DoCommand(di);
            return di.RowCount;
        }
        public virtual string LimitLoginTime(long uid)
        {
            DateTime limitTime = DateTime.Now;
            help.AddInParam("@UserId", MySqlDbType.Int64, uid);
            help.AddInParam("@ErrLogType", MySqlDbType.Int16, 0);
            help.AddInParam("@SuccessLogType", MySqlDbType.Int16, 1);
            help.AddInParam("@CreateDate", MySqlDbType.DateTime, DateTime.Now.AddHours(-24));

            DoQuerySql<SysLog> execsql = new DoQuerySql<SysLog>("select * from mz_sys_log where UserId=@UserId and (LogType=@ErrLogType or LogType=@SuccessLogType) and CreateDate>@CreateDate order by CreateDate desc limit 5");
            help.DoCommand(execsql);
            List<SysLog> sysloglist = execsql.ToList();
            int errcount = 0;
            foreach (SysLog lg in sysloglist)
            {
                if (lg.LogType == 1)
                {
                    break;
                }
                errcount++;
            }

            if (errcount == 3)
            {
                limitTime = sysloglist[0].CreateDate.AddMinutes(5);
            }
            else if (errcount == 4)
            {
                limitTime = sysloglist[0].CreateDate.AddHours(1);
            }
            else if (errcount >= 5)
            {
                limitTime = sysloglist[0].CreateDate.AddHours(24);
            }
            else
            {
                limitTime = DateTime.Now.AddMonths(-1);
            }

            TimeSpan ts = limitTime - DateTime.Now;
            if (ts.TotalSeconds > 0)
            {
                if (ts.TotalHours >= 1)
                {
                    return "帐户已经冻结。请" + (int)ts.TotalHours + "小时后再尝试登录";
                }
                else if (ts.TotalMinutes >= 1 && ts.TotalMinutes < 60)
                {
                    return "帐户已经冻结。请" + (int)ts.TotalMinutes + "分钟后再尝试登录";
                }
                else
                {
                    return "帐户已经冻结。请" + (int)ts.TotalSeconds + "秒后再尝试登录";
                }
            }
            return string.Empty;
        }

    }
}
