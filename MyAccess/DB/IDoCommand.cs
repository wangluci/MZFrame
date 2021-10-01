using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoCommand
    {
        /// <summary>
        /// 同步执行命令
        /// </summary>
        /// <param name="help"></param>
        void Excute(DbHelp help);
        /// <summary>
        /// 异步执行命令
        /// </summary>
        /// <param name="help"></param>
        /// <returns></returns>
        Task ExcuteAsync(DbHelp help);
    }
}
