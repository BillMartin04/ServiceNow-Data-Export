using ServiceNow_Data_Export.Interface;
using ServiceNow_Data_Export.Model;
using System.Net.Http.Headers;
using System.Text;

namespace ServiceNow_Data_Export.Service
{
    public sealed class ServiceNowCsvClient : IServiceNowCsvClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ServiceNowOptions _options;
        private readonly ILogger _logger;

        public ServiceNowCsvClient(ServiceNowOptions options, ILogger logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (string.IsNullOrWhiteSpace(_options.InstanceUrl))
                throw new InvalidOperationException("InstanceUrl is required in ServiceNowOptions.");

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(3)
            };

            // Basic auth
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ServiceNowCsvExporter/1.0");

            _logger.Info("ServiceNowCsvClient initialized.");
        }

        public async Task<DownloadResult> DownloadCsvAsync(ServiceNowCsvExportRequest request, CancellationToken ct = default)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var url = BuildCsvExportUrl(request);

            _logger.Info("Starting ServiceNow CSV download...");
            _logger.Info($"URL: {url}");
            _logger.Info($"Saving to: {request.LocalFilename}");

            EnsureDirectoryExists(request.LocalFilename);

            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);

            _logger.Info($"HTTP status: {(int)response.StatusCode} {response.ReasonPhrase}");

            if (!response.IsSuccessStatusCode)
            {
                var snippet = await SafeReadSnippetAsync(response, ct);
                throw new HttpRequestException($"ServiceNow export failed. Status={(int)response.StatusCode}. Snippet={snippet}");
            }

            var contentType = response.Content.Headers.ContentType?.MediaType;

            await using var remoteStream = await response.Content.ReadAsStreamAsync(ct);
            await using var localStream = new FileStream(
                request.LocalFilename,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

            var totalBytes = await CopyWithProgressAsync(remoteStream, localStream, ct);

            _logger.Info($"Completed. Bytes={totalBytes:N0}");

            return new DownloadResult(
                url: url,
                localFilename: request.LocalFilename,
                bytesDownloaded: totalBytes,
                statusCode: (int)response.StatusCode,
                contentType: contentType
            );
        }

        private string BuildCsvExportUrl(ServiceNowCsvExportRequest req)
        {
            var instance = _options.InstanceUrl.TrimEnd('/');
            var baseUrl = $"{instance}/{req.Table}_list.do?CSV";

            var query = $"&sysparm_query={Uri.EscapeDataString(req.EncodedQuery)}";

            var orderBy = string.IsNullOrWhiteSpace(req.OrderBy)
                ? ""
                : $"&sysparm_orderby={Uri.EscapeDataString(req.OrderBy!)}";

            // ✅ NEW: limit
            var limit = req.Limit.HasValue
                ? $"&sysparm_limit={req.Limit.Value}"
                : "";

            return baseUrl + query + orderBy + limit;
        }

        private void EnsureDirectoryExists(string localFilename)
        {
            var directory = Path.GetDirectoryName(localFilename);
            if (string.IsNullOrWhiteSpace(directory))
                return;

            Directory.CreateDirectory(directory);
            _logger.Info($"Ensured directory exists: {directory}");
        }

        private async Task<long> CopyWithProgressAsync(Stream source, Stream destination, CancellationToken ct)
        {
            var buffer = new byte[16 * 1024]; // 16KB
            long totalBytes = 0;

            long nextLogThreshold = 256 * 1024; // log every 256KB
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await destination.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                totalBytes += bytesRead;

                if (totalBytes >= nextLogThreshold)
                {
                    _logger.Info($"Downloaded {totalBytes:N0} bytes...");
                    nextLogThreshold += 256 * 1024;
                }
            }

            return totalBytes;
        }

        private static async Task<string> SafeReadSnippetAsync(HttpResponseMessage response, CancellationToken ct)
        {
            try
            {
                var text = await response.Content.ReadAsStringAsync(ct);
                if (string.IsNullOrWhiteSpace(text)) return "<empty>";
                return text.Length <= 300 ? text : text.Substring(0, 300);
            }
            catch
            {
                return "<unable to read response body>";
            }
        }

        public void Dispose()
        {
            _logger.Info("Disposing ServiceNowCsvClient.");
            _httpClient.Dispose();
        }
    }
}