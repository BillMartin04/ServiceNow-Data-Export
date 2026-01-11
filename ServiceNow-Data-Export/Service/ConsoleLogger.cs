using ServiceNow_Data_Export.Interface;

namespace ServiceNow_Data_Export.Service
{
    public sealed class ConsoleLogger : ILogger
    {
        public void Info(string message) => Write("INFO ", message);

        public void Warn(string message) => Write("WARN ", message);

        public void Error(string message, Exception? ex = null)
        {
            Write("ERROR", message);
            if (ex != null) Console.WriteLine(ex);
        }

        private static void Write(string level, string message)
        {
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {level} {message}");
        }
    }
}