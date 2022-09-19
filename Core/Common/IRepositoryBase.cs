using System.Threading.Tasks;

namespace Core.Common
{
    public interface IRepositoryBase
    {
        Task SaveTrackingChangesAsync();
    }
}
