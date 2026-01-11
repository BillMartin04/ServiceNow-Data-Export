using ServiceNow_Data_Export.Interface;
using ServiceNow_Data_Export.Model;

namespace ServiceNow_Data_Export.Service
{
    public sealed class CsvExportService : ICsvExportService
    {
        private readonly IServiceNowCsvClient _client;
        private readonly ILogger _logger;

        public CsvExportService(IServiceNowCsvClient client, ILogger logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<DownloadResult> ExportAsync(ServiceNowCsvExportRequest request, CancellationToken ct = default)
        {
            _logger.Info("CsvExportService invoked.");
            return await _client.DownloadCsvAsync(request, ct);
        }
    }
}