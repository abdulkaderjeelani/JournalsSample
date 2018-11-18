using Journals.Infrastructure.Interface;
using Journals.Repository;
using Journals.Repository.DTO;
using Journals.Services.ApplicationServices.Interface;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Journals.Services.ApplicationServices
{
    /*
     NOTE: If the application services involves only repository calls and no business logic, then
     creating a service layer will be a over head of extra layer.
     E.g. IMailServiceRepository
         */

    /// <summary>
    /// App service for journal subscriptions, as this is the core function of the domain, we use a service layer.
    /// </summary>
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IJournalRepository _journalRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(IJournalRepository journalRepository, ISubscriptionRepository subscriptionRepository)
        {
            this._journalRepository = journalRepository;
            this._subscriptionRepository = subscriptionRepository;
        }

        private IExceptionHandler _exceptionHandler;

        [Dependency]
        protected IExceptionHandler ExceptionHandler
        {
            get { return _exceptionHandler; }
            set { _exceptionHandler = value; }
        }

        public List<UserJournal> GetUserJournals(int userId)
        {
            List<UserJournal> userJournals = null;
            try
            {
                var journals = _journalRepository.GetAllJournals();

                if (journals != null && journals.Count > 0)
                {
                    var subscriptions = _subscriptionRepository.GetJournalsForSubscriber(userId);
                    userJournals = new List<UserJournal>();

                    journals.ForEach(j =>
                    {
                        userJournals.Add(new UserJournal
                        {
                            Content = j.Content,
                            ContentType = j.ContentType,
                            Description = j.Description,
                            FileName = j.FileName,
                            Id = j.Id,
                            JournalId = j.JournalId,
                            ModifiedDate = j.ModifiedDate,
                            Title = j.Title,
                            UserId = j.UserId,
                            IsSubscribed = (subscriptions.Any(k => k.JournalId == j.Id))
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }

            return userJournals;
        }

        public bool Subscribe(int journalId, int userId)
        {
            var opStatus = _subscriptionRepository.AddSubscription(journalId, userId);
            return opStatus == null ? false : opStatus.Status;
        }

        public bool UnSubscribe(int journalId, int userId)
        {
            var opStatus = _subscriptionRepository.RemoveSubscription(journalId, userId);
            return opStatus == null ? false : opStatus.Status;
        }
    }
}