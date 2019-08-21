using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Simple.Models
{
    public class Worker : BackgroundService
    {
        private const string Symbol = "CAD";
        private const int ThreadDelay = 5000;

        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;

        public Worker(ILogger<Worker> logger, IHttpClientFactory httpClient)
        {
            _logger = logger;
            _httpClient = httpClient.CreateClient();
            _serializer = new JsonSerializer();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    var response = await _httpClient.GetAsync($"https://api.exchangeratesapi.io/latest?base=USD&symbols={Symbol}", stoppingToken);
                    if (response.IsSuccessStatusCode == false)
                    {
                        _logger.LogCritical("Exchange Rate API Failed with HTTP Status Code {statusCode} at: {time}", response.StatusCode, DateTimeOffset.Now);
                        continue;
                    }

                    using var sr = new StreamReader(await response.Content.ReadAsStreamAsync());
                    using var jsonTextReader = new JsonTextReader(sr);
                    var exchangeRateResult = _serializer.Deserialize<CurrencyExchange>(jsonTextReader);

                    if (exchangeRateResult.Rates.TryGetValue(Symbol, out var cadValue))
                    {
                        _logger.LogInformation($"{Symbol} = {cadValue}");
                    }
                    else
                    {
                        _logger.LogCritical($"CAD Exchange rate not returned from API.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogCritical($"{nameof(HttpRequestException)}: {ex.Message}");
                }

                await Task.Delay(ThreadDelay, stoppingToken);
            }
        }
    }

    public class CurrencyExchange
    {
        public string Base { get; set; }
        public DateTime Date { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}