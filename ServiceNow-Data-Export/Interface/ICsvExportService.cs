using ServiceNow_Data_Export.Model;

namespace ServiceNow_Data_Export.Interface
{
    public interface ICsvExportService
    {
        Task<DownloadResult> ExportAsync(ServiceNowCsvExportRequest request, CancellationToken ct = default);
    }
}