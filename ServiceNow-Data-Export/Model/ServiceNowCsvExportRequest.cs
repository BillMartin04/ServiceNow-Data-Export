namespace ServiceNow_Data_Export.Model
{
    public sealed class ServiceNowCsvExportRequest
    {
        public string Table { get; }
        public string EncodedQuery { get; }
        public string? OrderBy { get; }
        public string LocalFilename { get; }

        // ✅ NEW: limit
        public int? Limit { get; }

        public ServiceNowCsvExportRequest(
            string table,
            string encodedQuery,
            string localFilename,
            string? orderBy = null,
            int? limit = null)
        {
            if (string.IsNullOrWhiteSpace(table))
                throw new ArgumentException("Table is required.", nameof(table));

            if (string.IsNullOrWhiteSpace(encodedQuery))
                throw new ArgumentException("EncodedQuery is required.", nameof(encodedQuery));

            if (string.IsNullOrWhiteSpace(localFilename))
                throw new ArgumentException("LocalFilename is required.", nameof(localFilename));

            if (limit.HasValue && limit.Value <= 0)
                throw new ArgumentException("Limit must be greater than 0 if provided.", nameof(limit));

            Table = table.Trim();
            EncodedQuery = encodedQuery.Trim();
            LocalFilename = localFilename.Trim();
            OrderBy = string.IsNullOrWhiteSpace(orderBy) ? null : orderBy.Trim();
            Limit = limit;
        }
    }
}