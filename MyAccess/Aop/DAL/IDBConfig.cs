using MyAccess.DB;
namespace MyAccess.Aop.DAL
{
    public interface IDBConfig
    {
        string Key { get; }
        IDbHelp CreateHelp();
    }
}
