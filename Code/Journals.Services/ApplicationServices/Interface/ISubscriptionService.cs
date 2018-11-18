using Journals.Repository.DTO;
using System.Collections.Generic;

namespace Journals.Services.ApplicationServices.Interface
{
    public interface ISubscriptionService
    {
        List<UserJournal> GetUserJournals(int userId);

        bool Subscribe(int journalId, int userId);

        bool UnSubscribe(int journalId, int userId);
    }
}