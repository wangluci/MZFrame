using System;
using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoCommand
    {
        void Excute(DbHelp help);
        /// <summary>
        /// 异步执行sql
        /// </summary>
        /// <param name="help"></param>
        /// <returns></returns>
        Task ExcuteAsync(DbHelp help);
    }
}
