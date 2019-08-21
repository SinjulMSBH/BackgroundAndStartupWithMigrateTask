using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class TaskExecutingServer : IServer
    {
        // Inject the original IServer implementation (KestrelServer) and
        // the list of IStartupTasks to execute
        private readonly IServer _server;
        private readonly IEnumerable<IStartupTask> _startupTasks;
        public TaskExecutingServer(IServer server, IEnumerable<IStartupTask> startupTasks)
        {
            _server = server;
            _startupTasks = startupTasks;
        }

        public async Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            // Run the tasks first
            foreach (var startupTask in _startupTasks)
            {
                await startupTask.ExecuteAsync(cancellationToken);
            }

            // Now start the Kestrel server properly
            await _server.StartAsync(application, cancellationToken);
        }

        // Delegate implementation to default IServer
        public IFeatureCollection Features => _server.Features;
        public void Dispose() => _server.Dispose();
        public Task StopAsync(CancellationToken cancellationToken) => _server.StopAsync(cancellationToken);
    }
}
