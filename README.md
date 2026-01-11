# ServiceNow-Data-Export

A C# (.NET) console application that exports ServiceNow list/table records to a CSV file using an encoded query, ordering, record limit, and runtime logging.

## What this project does

This tool downloads CSV export output from ServiceNow (`*_list.do?CSV`) and saves it locally. It is useful for basic reporting, testing, and simple automated exports.

## Requirements

- Visual Studio (recommended) or .NET SDK
- .NET 6+ (recommended)
- ServiceNow user account with permission to view/export the target table

## Configuration

Update the ServiceNow connection in `Program.cs`:

```csharp
var options = new ServiceNowOptions
{
    InstanceUrl = "https://YOUR_INSTANCE.service-now.com",
    Username = "YOUR_USERNAME",
    Password = "YOUR_PASSWORD"
};

Update the export request in Program.cs:

var request = new ServiceNowCsvExportRequest(
    table: "incident",
    encodedQuery: "priority=1",
    orderBy: "assigned_to",
    localFilename: @"c:\test\incident.csv"
);

Run the app

Using Visual Studio:

Click Run

Using terminal:

dotnet run


The CSV file will be saved to the output path you configured.

Notes

Export uses ServiceNow list CSV export (*_list.do?CSV)

Record limiting uses sysparm_record_count

Paging can use sysparm_first_row (optional)

ServiceNow export permissions and instance security settings may affect results

Security Warning

Do NOT commit credentials to GitHub. Use environment variables or a local config file that is excluded via .gitignore.

Disclaimer

For learning and internal automation only. Always follow your company’s security/compliance rules when exporting ServiceNow data.
