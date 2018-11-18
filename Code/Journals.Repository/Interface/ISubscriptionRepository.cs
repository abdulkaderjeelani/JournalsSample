using Journals.Model;
using System.Collections.Generic;

namespace Journals.Repository
{
    public interface ISubscriptionRepository
    {
        List<Subscription> GetJournalsForSubscriber(int userId);

        OperationStatus AddSubscription(int journalId, int userId);

        OperationStatus RemoveSubscription(int journalId, int userId);
    }
}