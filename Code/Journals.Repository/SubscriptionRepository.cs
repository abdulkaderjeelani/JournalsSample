using Journals.Model;
using Journals.Repository.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journals.Repository
{
    public class SubscriptionRepository : RepositoryBase<JournalsContext>, ISubscriptionRepository
    {
        public SubscriptionRepository()
        {
        }

        public SubscriptionRepository(JournalsContext context)
        {
            this.DataContext = context;
        }

        public List<Subscription> GetJournalsForSubscriber(int userId)
        {
            List<Subscription> list = null;
            try
            {
                using (this)
                    list = GetQueryable<Subscription>(u => u.UserId == userId).ToList();
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }

            return list;
        }

        public OperationStatus AddSubscription(int journalId, int userId)
        {
            var opStatus = new OperationStatus { Status = true };
            try
            {
                using (DataContext)
                {
                    Subscription s = new Subscription();
                    s.JournalId = journalId;
                    s.UserId = userId;
                    var j = DataContext.Subscriptions.Add(s);
                    DataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                opStatus.CreateFromException("Error adding subscription: ", e);
                ExceptionHandler.HandleException(e);
            }

            return opStatus;
        }

        public OperationStatus RemoveSubscription(int journalId, int userId)
        {
            var opStatus = new OperationStatus { Status = true };
            try
            {
                using (DataContext)
                {
                    var subscriptions = DataContext.Subscriptions.Where(u => u.JournalId == journalId && u.UserId == userId);

                    foreach (var s in subscriptions)
                    {
                        DataContext.Subscriptions.Remove(s);
                    }
                    DataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                opStatus.CreateFromException("Error deleting subscription: ", e);
                ExceptionHandler.HandleException(e);
            }

            return opStatus;
        }
    }
}