using ServiceNow_Data_Export.Model;

namespace ServiceNow_Data_Export.Interface
{
    public interface IServiceNowCsvClient : IDisposable
    {
        Task<DownloadResult> DownloadCsvAsync(ServiceNowCsvExportRequest request, CancellationToken ct = default);
    }
}