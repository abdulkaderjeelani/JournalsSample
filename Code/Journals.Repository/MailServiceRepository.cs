using Journals.Repository.DataContext;
using Journals.Repository.DTO;
using Journals.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journals.Repository
{
    public class MailServiceRepository : RepositoryBase<JournalsContext>, IMailServiceRepository
    {
        public MailServiceRepository()
        {
        }

        public MailServiceRepository(JournalsContext context)
        {
            this.DataContext = context;
        }

        public List<IssueDigestMail> GetIssuesForDigestMail(DateTime date)
        {
            List<IssueDigestMail> list = null;
            try
            {
                var issues = DataContext.Journals.Include("ParentJournal").Where(j => j.JournalId > 0);
                var subscriptions = DataContext.Subscriptions.Include("User");

                var frm = date.Date;
                var to = date.Date.AddDays(1);

                var issuesOnDate = (from s in subscriptions
                                    join i in issues on s.JournalId equals i.JournalId
                                    where i.ModifiedDate >= frm && i.ModifiedDate < to
                                    select new IssueDigestMail
                                    {
                                        UserId = s.User.UserId,
                                        EmailId = s.User.EmailId,
                                        Username = s.User.UserName,
                                        IssueTitle = i.Title,
                                        JournalTitle = i.ParentJournal.Title,
                                        JournalId = i.JournalId.Value,
                                        IssueDate = i.ModifiedDate
                                    });

                list = issuesOnDate.ToList();
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }
            return list;
        }
    }
}