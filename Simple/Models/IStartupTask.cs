using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
