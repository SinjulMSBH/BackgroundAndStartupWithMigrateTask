using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class TcpEchoServer : BackgroundService
    {
        private readonly ILogger<TcpEchoServer> _logger;
        private readonly IConnectionListenerFactory _factory;
        private IConnectionListener _listener;

        public TcpEchoServer(ILogger<TcpEchoServer> logger, IConnectionListenerFactory factory)
        {
            _logger = logger;
            _factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _listener = await _factory.BindAsync(new IPEndPoint(IPAddress.Loopback, 6000), stoppingToken);

            while (true)
            {
                var connection = await _listener.AcceptAsync(stoppingToken);
                // AcceptAsync will return null upon disposing the listener
                if (connection == null)
                {
                    break;
                }
                // In an actual server, ensure all accepted connections are disposed prior to completing
                _ = Echo(connection, stoppingToken);
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _listener.DisposeAsync();
        }

        private async Task Echo(ConnectionContext connection, CancellationToken stoppingToken)
        {
            try
            {
                var input = connection.Transport.Input;
                var output = connection.Transport.Output;

                await input.CopyToAsync(output, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Connection {ConnectionId} cancelled due to server shutdown", connection.ConnectionId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Connection {ConnectionId} threw an exception", connection.ConnectionId);
            }
            finally
            {
                await connection.DisposeAsync();
                _logger.LogInformation("Connection {ConnectionId} disconnected", connection.ConnectionId);
            }
        }
    }
}
