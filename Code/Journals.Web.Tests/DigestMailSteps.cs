using Journals.DigestMailService;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace Journals.Web.Tests
{
    [Binding]
    public class DigestMailSteps
    {
        private DigestMailer mailer = null;
        private Func<List<int>> onReturnAlreadyNoifiedUsers = null;
        private Action<List<int>> onCallSaveNotified = null;
        private IJournalMock<IMailer> mockMailer = null;
        private List<int> NotifiedUsersStub = null;
        private List<int> NewlyNotifiedUsersStub = null;

        private UserProfile testUser = JournalMocks.UserData[0];
        private Journal subscribedJournal = JournalMocks.JournalData[0];
        private Journal notSubscribedJournal = JournalMocks.JournalData[3];

        private Dictionary<string, string> mailToAndBody = new Dictionary<string, string>();

        private bool isInit = false;

        public void Init()
        {
            NewlyNotifiedUsersStub = new List<int>();

            if (isInit) return;

            onReturnAlreadyNoifiedUsers = () =>
            {
                return NotifiedUsersStub;
            };
            onCallSaveNotified = (list) =>
            {
                NewlyNotifiedUsersStub.AddRange(list);
            };

            mockMailer = JournalMocks.MockMailer((from, to, sub, body) =>
            {
                //1 mail per user so key must be unique, if there is exception here then we knew somethign is wrong
                mailToAndBody.Add(to, body);
            });

            mailer = new DigestMailer(
                JournalMocks.MockMailServiceRepositoryWithContext(),
                mockMailer.MockObject,
                JournalMocks.MockExceptionHandler().MockObject,
                JournalMocks.MockNotificationRepository(onReturnAlreadyNoifiedUsers, onCallSaveNotified).MockObject);
            isInit = true;
        }

        [Given(@"A user has already subscribed to some journals")]
        public void GivenAUserHasAlreadySubscribedToSomeJournals()
        {
            Init();

            //remove the previous subscription to avoid duplicate subscription
            var duplicates = JournalMocks.SubscriptionData.Where(s => s.UserId == testUser.UserId && s.JournalId == subscribedJournal.Id);
            if (duplicates.Any())
                JournalMocks.SubscriptionData = JournalMocks.SubscriptionData.Except(duplicates).ToList();

            JournalMocks.SubscriptionData.Add(new Subscription
            {
                Id = 101,
                UserId = testUser.UserId,
                User = testUser,
                JournalId = subscribedJournal.Id,
                Journal = subscribedJournal,
            });
        }

        [Given(@"I have created a new issue of a journal that user has subscribed to")]
        public void GivenIHaveCreatedANewIssueOfAJournalThatUserHasSubscribedTo()
        {
            Init();
            JournalMocks.JournalData.Add(new Journal
            {
                Id = 500,
                Title = "mail_issue_subscription",
                ModifiedDate = DateTime.Now,
                JournalId = subscribedJournal.Id,
                ParentJournal = subscribedJournal
            });
        }

        [Given(@"I have created a new issue of a journal that user has NOT subscribed to")]
        public void GivenIHaveCreatedANewIssueOfAJournalThatUserHasNOTSubscribedTo()
        {
            Init();

            //to remove the subs if any for our test user
            var nonSubs = JournalMocks.SubscriptionData.Where(s => s.UserId == testUser.UserId && s.JournalId == notSubscribedJournal.JournalId);
            if (nonSubs.Any())
                JournalMocks.SubscriptionData = JournalMocks.SubscriptionData.Except(nonSubs).ToList();

            JournalMocks.JournalData.Add(new Journal
            {
                Id = 501,
                Title = "mail_issue_no_subscription",
                ModifiedDate = DateTime.Now,
                JournalId = notSubscribedJournal.Id,
                ParentJournal = notSubscribedJournal
            });
        }

        [Given(@"the mail is already sent for the day")]
        public void GivenTheMailIsAlreadySentForTheDay()
        {
            NotifiedUsersStub = new List<int>();
            NotifiedUsersStub.Add(testUser.UserId);
        }

        [When(@"The mail service runs")]
        public void WhenTheMailServiceRuns()
        {
            mailer.Run(DateTime.Now);
        }

        [Then(@"the issue should be notified in email")]
        public void ThenTheIssueShouldBeNotifiedInEmail()
        {
            Assert.IsTrue(mailToAndBody.ContainsKey(testUser.EmailId) &&
                mailToAndBody[testUser.EmailId].Contains("mail_issue_subscription"));
        }

        [Then(@"the issue should NOT be notified in email")]
        public void ThenTheIssueShouldNOTBeNotifiedInEmail()
        {
            //mail is not sent if he has no other journals apart from our non subscribed journal, or will be sent only for the journals he subscribed to
            Assert.IsTrue(!mailToAndBody.ContainsKey(testUser.EmailId) ||
                !mailToAndBody[testUser.EmailId].Contains("mail_issue_no_subscription"));
        }
    }
}