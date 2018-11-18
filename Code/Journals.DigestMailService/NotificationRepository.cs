using Journals.DigestMailService.Interface;
using Journals.Infrastructure.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Journals.DigestMailService
{
    internal class NotificationRepository : INotificationRepository
    {
        private readonly IExceptionHandler _exHanlder = null;

        public NotificationRepository(IExceptionHandler exHanlder)
        {
            _exHanlder = exHanlder;
        }

        public void SaveNotifiedUsers(DateTime mailDate, List<int> mailSentUsers)
        {
            /*Uses JSON flat file storage, to persist sent mails, while we scale we can improve this to own db (sqlite).
             Assumptions, All digest mail per day are sent in a single batch i.e. all mails are sent in 1 shot
             */
            string mailLogFile = GetLogFileName(mailDate);

            using (StreamWriter file = File.CreateText(mailLogFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, mailSentUsers);
            }
        }

        public List<int> GetAlreadyNotifiedUsers(DateTime mailDate)
        {
            List<int> notifiedUsers = null;

            string mailLogFile = GetLogFileName(mailDate);
            if (File.Exists(mailLogFile))
                using (StreamReader file = File.OpenText(mailLogFile))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    notifiedUsers = (List<int>)serializer.Deserialize(file, typeof(List<int>));
                }
            return notifiedUsers;
        }

        public void ClearOldNotifications(DateTime mailDate)
        {
            const int noOfDaysToLog = 3;
            Parallel.For(1, noOfDaysToLog, day =>
            {
                var dataTodeleteLog = mailDate.AddDays(-1 * day);
                string fileToDelete = GetLogFileName(dataTodeleteLog);
                try
                {
                    System.IO.File.Delete(fileToDelete);
                }
                catch (Exception deleteEx)
                {
                    //log here so that the loop does not terminate
                    _exHanlder.HandleException(deleteEx);
                }
            });
        }

        /// <summary>
        /// Gets the log filename to create / delete
        /// </summary>
        /// <param name="mailDate"></param>
        /// <returns></returns>
        private string GetLogFileName(DateTime mailDate)
        {
            string executingPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            const string folderName = "MailLogs";
            string folder = Path.Combine(executingPath, folderName);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            string fileName = string.Concat(mailDate.ToString("dd MMM yyyy"), ".json");
            return Path.Combine(folder, fileName);
        }
    }
}