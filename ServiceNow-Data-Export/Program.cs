using ServiceNow_Data_Export.Controller;
using ServiceNow_Data_Export.Interface;
using ServiceNow_Data_Export.Model;
using ServiceNow_Data_Export.Service;

namespace ServiceNow_Data_Export
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();

            // Config (can move to appsettings.json later if you want)
            var options = new ServiceNowOptions
            {
                InstanceUrl = "https://dev328600.service-now.com",
                Username = "admin",
                Password = "Gwy1I$aqZ6M!"
            };

            var request = new ServiceNowCsvExportRequest(
                table: "incident",
                encodedQuery: "priority=1",
                orderBy: "assigned_to",
                localFilename: @"c:\test\incident.csv",
                limit: 5

            );

            // SOA wiring (manual DI)
            using IServiceNowCsvClient client = new ServiceNowCsvClient(options, logger);
            ICsvExportService exportService = new CsvExportService(client, logger);
            var controller = new CsvExportController(exportService, logger);

            try
            {
                await controller.RunAsync(request);
                logger.Info("Program finished.");
            }
            catch (Exception ex)
            {
                logger.Error("Program failed.", ex);
            }
        }
    }
}
