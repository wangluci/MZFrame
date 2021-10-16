using System.Threading.Tasks;

namespace MyAccess.DB
{
    public interface IDoResult<T>
    {
        void SetResult(T result);
        Task SetResultAsync(T result);
    }
}
