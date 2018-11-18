using log4net;
using System;
using System.Threading.Tasks;

namespace Journals.Infrastructure
{
    /// <summary>
    /// A simple class for exception logging using log4net,
    /// </summary>
    public class LogExceptionHandler : Journals.Infrastructure.Interface.IExceptionHandler
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(LogExceptionHandler));

        public void HandleException(Exception ex)
        {
            //let the current thread wait till the log task is done, TODO: Add external wait mechanism
            Task.Run(() => LogException(ex)).Wait();
        }

        private void LogException(Exception ex)
        {
            log.Fatal(ex.Message, ex);
        }
    }
}