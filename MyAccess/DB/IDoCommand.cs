using System;

namespace MyAccess.DB
{
    public interface IDoCommand
    {
        void Excute(DbHelp help);
    }
}
