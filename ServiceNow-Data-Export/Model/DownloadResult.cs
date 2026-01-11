

namespace ServiceNow_Data_Export.Model
{
    public sealed class DownloadResult
    {
        public string Url { get; }
        public string LocalFilename { get; }
        public long BytesDownloaded { get; }
        public int StatusCode { get; }
        public string? ContentType { get; }

        public DownloadResult(string url, string localFilename, long bytesDownloaded, int statusCode, string? contentType)
        {
            Url = url;
            LocalFilename = localFilename;
            BytesDownloaded = bytesDownloaded;
            StatusCode = statusCode;
            ContentType = contentType;
        }
    }
}
