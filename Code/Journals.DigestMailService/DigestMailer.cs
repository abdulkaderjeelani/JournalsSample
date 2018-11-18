using Journals.DigestMailService.Interface;
using Journals.Infrastructure.Interface;
using Journals.Repository.DTO;
using Journals.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journals.DigestMailService
{
    public class DigestMailer
    {
        //todo: read from config
        private static readonly string FromJournalMail = "journalmaster@crossover.com";

        private readonly IMailServiceRepository _mailRepository = null;
        private readonly IMailer _mailer = null;
        private readonly IExceptionHandler _exHanlder = null;
        private readonly INotificationRepository _notificationRepository = null;

        public DigestMailer(IMailServiceRepository mailRepository, IMailer mailer,
                            IExceptionHandler exHanlder, INotificationRepository notificationRepository)
        {
            _mailRepository = mailRepository;
            _mailer = mailer;
            _exHanlder = exHanlder;
            _notificationRepository = notificationRepository;
        }

        /// <summary>
        /// Gets the mailing digest list , construct and send emails.
        /// </summary>
        /// <param name="dateOfMailing"></param>
        public void Run(DateTime? dateOfMailing = null)
        {
            /*configure the mail program on scheduler to run on early morning, so that all of previous day issues are sent out.
             Things to consider: Mail specific to time zone.
             * To capture user timezone on register, and use that to send out emails, so that all of our users
             * receive mail at 6am on their specific timezones.
             * OR use UTC fso that all users will receive mails at specific UTC time (Not so good, but easy to implement)
             */

            var mailDate = (dateOfMailing.HasValue) ? dateOfMailing.Value : DateTime.Now.Date.AddDays(-1);

            var clearLogTask = Task.Run(() => _notificationRepository.ClearOldNotifications(mailDate));

            List<IssueDigestMail> issueDigestMails = GetDigestMails(mailDate);

            if (issueDigestMails != null && issueDigestMails.Count > 0)
            {
                List<Task> mailSendingTasks = new List<Task>();
                List<int> mailSentUsers = new List<int>();

                var issesByUser = issueDigestMails.GroupBy(u => new UserGroupingKey(u.UserId, u.Username, u.EmailId));

                foreach (var userIssues in issesByUser)
                {
                    mailSendingTasks.Add(SendMailToUser(userIssues));
                    mailSentUsers.Add(userIssues.Key.UserId);
                }

                _notificationRepository.SaveNotifiedUsers(mailDate, mailSentUsers);

                //wait for our tasks
                Task.WaitAll(mailSendingTasks.ToArray());
            }

            clearLogTask.Wait();
        }

        /// <summary>
        /// Gets the mails to send for the day.
        /// </summary>
        /// <param name="mailDate"></param>
        /// <returns></returns>
        private List<IssueDigestMail> GetDigestMails(DateTime mailDate)
        {
            var issueDigestMails = _mailRepository.GetIssuesForDigestMail(mailDate);
            var alreadyNotifiedUsers = _notificationRepository.GetAlreadyNotifiedUsers(mailDate);

            if (alreadyNotifiedUsers != null && alreadyNotifiedUsers.Count > 0)
                issueDigestMails.RemoveAll(issue => alreadyNotifiedUsers.Contains(issue.UserId));
            return issueDigestMails;
        }

        /// <summary>
        /// Builds the mail and sends out to user
        /// </summary>
        /// <param name="userIssues"></param>
        /// <returns></returns>
        private Task SendMailToUser(IGrouping<UserGroupingKey, IssueDigestMail> userIssues)
        {
            Task mailTask = null;
            try
            {
                StringBuilder mailBodyBuilder = new StringBuilder();
                mailBodyBuilder.AppendFormat("Hi {0} here are your new issues", userIssues.Key.Username);

                var userIssuesByJournal = userIssues.GroupBy(j => new { j.JournalId, j.JournalTitle });

                foreach (var journalIssue in userIssuesByJournal)
                {
                    mailBodyBuilder.AppendLine();
                    mailBodyBuilder.AppendFormat("Journal {0} : ", journalIssue.Key.JournalTitle);

                    foreach (var newIssues in journalIssue)
                    {
                        mailBodyBuilder.AppendLine();
                        mailBodyBuilder.AppendFormat("Title {0}, Published On {1} ", newIssues.IssueTitle, newIssues.IssueDate.ToString("dd MMM yyyy HH:mm"));
                    }
                }

                mailBodyBuilder.AppendLine("Thanks,");
                mailBodyBuilder.AppendLine("Journals Team.");

                string subject = "New issues were published for your subscribed journals.";
                string body = mailBodyBuilder.ToString();
                string email = userIssues.Key.EmailId;
                mailTask = Task.Run(() => _mailer.SendMail(FromJournalMail, email, subject, body));
            }
            catch (Exception userMailEx)
            {
                //handle here so if there is any failure for user then  the loop continues
                _exHanlder.HandleException(userMailEx);
            }
            return mailTask;
        }

        private struct UserGroupingKey
        {
            public UserGroupingKey(int userId, string userName, string emailId)
            {
                this.UserId = userId;
                this.Username = userName;
                this.EmailId = emailId;
            }

            public int UserId { get; private set; }
            public string Username { get; private set; }
            public string EmailId { get; private set; }
        }
    }
}