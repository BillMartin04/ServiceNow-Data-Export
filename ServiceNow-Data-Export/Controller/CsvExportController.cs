using ServiceNow_Data_Export.Interface;
using ServiceNow_Data_Export.Model;

namespace ServiceNow_Data_Export.Controller
{
    public sealed class CsvExportController
    {
        private readonly ICsvExportService _exportService;
        private readonly ILogger _logger;

        public CsvExportController(ICsvExportService exportService, ILogger logger)
        {
            _exportService = exportService ?? throw new ArgumentNullException(nameof(exportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RunAsync(ServiceNowCsvExportRequest request, CancellationToken ct = default)
        {
            _logger.Info("CsvExportController started...");

            var result = await _exportService.ExportAsync(request, ct);

            _logger.Info("Export completed successfully.");
            _logger.Info($"Saved file: {result.LocalFilename}");
            _logger.Info($"Bytes: {result.BytesDownloaded:N0}");
            _logger.Info($"HTTP Status: {result.StatusCode}");
            _logger.Info($"Content-Type: {result.ContentType ?? "(unknown)"}");
        }
    }
}