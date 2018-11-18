using System;
using System.Collections.Generic;

namespace Journals.DigestMailService.Interface
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Mark the mails as sent, so that if the process runs again on same date we ignore these.
        /// </summary>
        /// <param name="mailDate"></param>
        /// <param name="mailSentUsers">ids of users which the mail has sent</param>
        void SaveNotifiedUsers(DateTime mailDate, List<int> mailSentUsers);

        /// <summary>
        /// Gets the users to whom the mails are sent on given date
        /// </summary>
        /// <param name="mailDate"></param>
        /// <returns></returns>
        List<int> GetAlreadyNotifiedUsers(DateTime mailDate);

        /// <summary>
        /// Clears the old notifications prior to given mail date
        /// </summary>
        /// <param name="mailDate"></param>
        /// <returns></returns>
        void ClearOldNotifications(DateTime mailDate);
    }
}