using Jourals.Web;
using Journals.DigestMailService.Interface;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository;
using Journals.Repository.DataContext;
using Journals.Repository.Interface;
using Journals.Services.ApplicationServices;
using Journals.Services.ApplicationServices.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Journals.Web.Tests
{
    /// <summary>
    /// Create a separate factory class for Mocks used, as the application is simple we are putting all mocks inside this class.
    /// Once it grows create separate classes for RepositoryMocks, ServiceMocks etc... for horizontal separation (mock per concern)
    /// Create 1 Mock per Boundedcontext / logical item for vertical separation. (mock per logical item)
    /// </summary>
    internal class JournalMocks
    {
        /// <summary>
        /// Type initializer, all our tests use this mocking class so any global initialization can also be done here !
        /// </summary>
        static JournalMocks()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        internal static UserProfile StubUserProfile()
        {
            return new UserProfile { UserId = 1 };
        }

        internal static IJournalMock<IJournalRepository> MockJournalRepository(UserProfile profile)
        {
            var jourRepoMock = new Mock<IJournalRepository>();
            jourRepoMock.Setup(jr => jr.GetAllJournals(profile.UserId)).Returns(() => JournalData.Where(j => j.JournalId == null || j.JournalId == 0).ToList());
            jourRepoMock.Setup(cr => cr.AddJournal(It.IsAny<Journal>())).Returns(new OperationStatus { Status = true });
            return new MOQMock<IJournalRepository>(jourRepoMock);
        }

        internal static IJournalMock<IStaticMembershipService> MockIStaticMembershipService(UserProfile profile)
        {
            var memServiceMock = new Mock<IStaticMembershipService>();
            memServiceMock.Setup(ms => ms.GetUser()).Returns(profile);
            return new MOQMock<IStaticMembershipService>(memServiceMock);
        }

        internal static IJournalMock<ISessionProvider> MockISessionProvider(UserProfile profile)
        {
            var sessionProvider = new Mock<ISessionProvider>();
            sessionProvider.SetupGet(s => s["LoggedInUser"]).Returns(profile);
            return new MOQMock<ISessionProvider>(sessionProvider);
        }

        internal static IJournalMock<HttpPostedFileBase> MockFile()
        {
            return new MOQMock<HttpPostedFileBase>(new Mock<HttpPostedFileBase>());
        }

        internal static Mock<DbSet<Journal>> MockDbSetJournal()
        {
            var mockDbSetJournal = new Mock<DbSet<Journal>>();
            //Create a fluent class if needed, ALWAYS USE THE FUNC<> OVER LOAD TO REFLECT THE CHANGES IN UNDERLYING DATA
            mockDbSetJournal.As<IQueryable<Journal>>().Setup(m => m.Provider).Returns(() =>
            {
                return JournalData.AsQueryable().Provider;
            });
            mockDbSetJournal.As<IQueryable<Journal>>().Setup(m => m.Expression).Returns(() =>
            {
                return JournalData.AsQueryable().Expression;
            });
            mockDbSetJournal.As<IQueryable<Journal>>().Setup(m => m.ElementType).Returns(() =>
            {
                return JournalData.AsQueryable().ElementType;
            });
            mockDbSetJournal.As<IQueryable<Journal>>().Setup(m => m.GetEnumerator()).Returns(() =>
            {
                return JournalData.AsQueryable().GetEnumerator();
            });

            mockDbSetJournal.Setup(m => m.Include(It.IsAny<string>())).Returns(() =>
            {
                return mockDbSetJournal.Object;
            });

            var captured = mockDbSetJournal;

            mockDbSetJournal.Setup(m => m.Add(It.IsAny<Journal>())).Returns<Journal>(j =>
            {
                JournalData.Add(j);
                return new Journal { Id = captured.Object.Count() + 1 };
            });

            return mockDbSetJournal;
        }

        internal static Mock<DbSet<Subscription>> MockDbSetSubscription()
        {
            var mockDbSetSubscription = new Mock<DbSet<Subscription>>();
            //Create a fluent class if needed, ALWAYS USE THE FUNC<> OVER LOAD TO REFLECT THE CHANGES IN UNDERLYING DATA
            mockDbSetSubscription.As<IQueryable<Subscription>>().Setup(m => m.Provider).Returns(() =>
            {
                return SubscriptionData.AsQueryable().Provider;
            });
            mockDbSetSubscription.As<IQueryable<Subscription>>().Setup(m => m.Expression).Returns(() =>
            {
                return SubscriptionData.AsQueryable().Expression;
            });
            mockDbSetSubscription.As<IQueryable<Subscription>>().Setup(m => m.ElementType).Returns(() =>
            {
                return SubscriptionData.AsQueryable().ElementType;
            });
            mockDbSetSubscription.As<IQueryable<Subscription>>().Setup(m => m.GetEnumerator()).Returns(() =>
            {
                return SubscriptionData.AsQueryable().GetEnumerator();
            });

            mockDbSetSubscription.Setup(m => m.Include(It.IsAny<string>())).Returns(() =>
            {
                return mockDbSetSubscription.Object;
            });

            var captured = mockDbSetSubscription;

            mockDbSetSubscription.Setup(m => m.Add(It.IsAny<Subscription>())).Returns<Subscription>(s =>
            {
                SubscriptionData.Add(s);
                return new Subscription { Id = captured.Object.Count() + 1 };
            });

            mockDbSetSubscription.Setup(m => m.Remove(It.IsAny<Subscription>())).Returns<Subscription>(s =>
            {
                SubscriptionData.Remove(s);
                return new Subscription { Id = captured.Object.Count() + 1 };
            });

            return mockDbSetSubscription;
        }

        internal static Mock<JournalsContext> MockContext(Mock<DbSet<Journal>> mockDbSetJournal = null, Mock<DbSet<Subscription>> mockDbSetSubscription = null)
        {
            if (mockDbSetJournal == null)
                mockDbSetJournal = MockDbSetJournal();

            if (mockDbSetSubscription == null)
                mockDbSetSubscription = MockDbSetSubscription();

            var mockContext = new Mock<JournalsContext>();

            if (mockDbSetJournal != null)
            {
                var dbSetJournal = mockDbSetJournal.Object;
                if (dbSetJournal != null)
                {
                    mockContext.Setup(c => c.Journals).Returns(() =>
                    {
                        return dbSetJournal;
                    });
                    mockContext.Setup(c => c.Set<Journal>()).Returns(() =>
                    {
                        return dbSetJournal;
                    });
                }
            }

            if (mockDbSetSubscription != null)
            {
                var dbSetSubscription = mockDbSetSubscription.Object;
                if (dbSetSubscription != null)
                {
                    mockContext.Setup(c => c.Subscriptions).Returns(() =>
                    {
                        return dbSetSubscription;
                    });
                    mockContext.Setup(c => c.Set<Subscription>()).Returns(() =>
                    {
                        return dbSetSubscription;
                    });
                }
            }
            return mockContext;
        }

        internal static IJournalRepository MockJournalRepositoryWithContext()
        {
            Mock<JournalsContext> mockJournalContext = MockContext();
            var journalRepository = new JournalRepository(mockJournalContext.Object);
            journalRepository.SetExceptionHandler(MockExceptionHandler().MockObject);
            return journalRepository;
        }

        internal static ISubscriptionService MockSubscriptionServiceWithContext()
        {
            Mock<JournalsContext> mockJournalContext = MockContext();
            var journalRepository = new JournalRepository(mockJournalContext.Object);
            SetExceptionHandlerForRepository(journalRepository);

            var subscriptionRepository = new SubscriptionRepository(mockJournalContext.Object);
            SetExceptionHandlerForRepository(subscriptionRepository);

            var subscriptionService = new SubscriptionService(journalRepository, subscriptionRepository);
            return subscriptionService;
        }

        internal static IMailServiceRepository MockMailServiceRepositoryWithContext()
        {
            Mock<JournalsContext> mockJournalContext = MockContext();
            var mailServiceRepository = new MailServiceRepository(mockJournalContext.Object);
            SetExceptionHandlerForRepository(mailServiceRepository);
            return mailServiceRepository;
        }

        private static void SetExceptionHandlerForRepository(RepositoryBase<JournalsContext> repository)
        {
            repository.SetExceptionHandler(MockExceptionHandler().MockObject);
        }

        internal static IJournalMock<IMailer> MockMailer(Action<string, string, string, string> mailSentCallback)
        {
            var mock = new Mock<IMailer>();
            mock.Setup(m => m.SendMail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string, string>((from, to, sub, body) =>
                {
                    mailSentCallback(from, to, sub, body);
                }).Verifiable();
            return new MOQMock<IMailer>(mock);
        }

        internal static IJournalMock<IExceptionHandler> MockExceptionHandler()
        {
            var mock = new Mock<IExceptionHandler>();
            mock.Setup(m => m.HandleException(It.IsAny<Exception>())).Verifiable();
            return new MOQMock<IExceptionHandler>(mock);
        }

        internal static IJournalMock<INotificationRepository> MockNotificationRepository(Func<List<int>> onReturnAlreadyNoifiedUsers, Action<List<int>> onCallSaveNotified)
        {
            var mock = new Mock<INotificationRepository>();
            mock.Setup(m => m.ClearOldNotifications(It.IsAny<DateTime>())).Verifiable();
            mock.Setup(m => m.GetAlreadyNotifiedUsers(It.IsAny<DateTime>())).Returns(onReturnAlreadyNoifiedUsers);
            mock.Setup(m => m.SaveNotifiedUsers(It.IsAny<DateTime>(), It.IsAny<List<int>>())).Callback<DateTime, List<int>>((date, list) => onCallSaveNotified(list));
            return new MOQMock<INotificationRepository>(mock);
        }

        internal static List<UserProfile> UserData = GenerateUsersStub();

        internal static List<Journal> JournalData = GenerateJournalStub();

        internal static List<Subscription> SubscriptionData = GenerateSubscriptionStub();

        internal static List<UserProfile> GenerateUsersStub()
        {
            var userList = new List<UserProfile>();
            for (int i = 1; i <= 6; i++)
            {
                userList.Add(new UserProfile
                {
                    UserId = i,
                    UserName = string.Format("name_of_{0}", i),
                    EmailId = string.Format("email_of_{0}@jtest.com", i)
                });
            }
            return userList;
        }

        private static List<Journal> GenerateJournalStub()
        {
            var journals = new List<Journal>() {
                new Journal { Id =1, Title= "Journal 1", FileName ="f1", UserId = UserData[0].UserId, User = UserData[0] ,JournalId = null , ModifiedDate = DateTime.Now },
                new Journal { Id =2, Title= "Journal 2", FileName ="f2", UserId = UserData[0].UserId, User = UserData[0] ,JournalId = null  , ModifiedDate = DateTime.Now},
                new Journal { Id =3, Title= "Journal 3", FileName ="f2", UserId = UserData[0].UserId, User = UserData[0] , JournalId = null , ModifiedDate = DateTime.Now },
                new Journal { Id =4, Title= "Journal 4", FileName ="f2", UserId = UserData[0].UserId, User = UserData[0] ,JournalId = null , ModifiedDate = DateTime.Now }
            };

            var issues = new List<Journal>()
            {
                new Journal { Id =5, Title= "Issue 1 for Journal 1", FileName ="f21", UserId =UserData[0].UserId, User = UserData[0] , JournalId = journals[0].Id, ParentJournal = journals[0], ModifiedDate = DateTime.Now},
                new Journal { Id =6, Title= "Issue 2 for Journal 1", FileName ="f22", UserId =UserData[0].UserId, User = UserData[0] , JournalId = journals[0].Id, ParentJournal = journals[0], ModifiedDate = DateTime.Now},
                new Journal { Id =7, Title= "Issue 1 for Journal 2", FileName ="f11", UserId =UserData[0].UserId, User = UserData[0] ,JournalId = journals[1].Id, ParentJournal = journals[1], ModifiedDate = DateTime.Now},
            };

            journals.AddRange(issues);
            return journals;
        }

        private static List<Subscription> GenerateSubscriptionStub()
        {
            var user1 = UserData[0];
            var user5 = UserData[4];
            var user6 = UserData[5];

            var j1 = JournalData[0];
            var j2 = JournalData[1];
            var j3 = JournalData[2];
            var j4 = JournalData[3];

            return new List<Subscription>() {
              new Subscription { Id = 1, UserId = user1.UserId, JournalId =j1.Id, Journal = j1, User = user1 },
              new Subscription { Id = 2, UserId = user1.UserId, JournalId =j2.Id, Journal = j2, User = user1 },
              new Subscription { Id = 3, UserId = user1.UserId, JournalId =j3.Id, Journal = j3, User = user1 },

              new Subscription { Id = 4, UserId = user5.UserId, JournalId =j1.Id, Journal = j1, User = user5 },
              new Subscription { Id = 5, UserId = user5.UserId, JournalId =j3.Id, Journal = j3, User = user5 },

              new Subscription { Id = 6, UserId = user6.UserId, JournalId =j4.Id, Journal = j4, User = user6 },
            };
        }
    }

    /// <summary>
    /// Mock interface for our testing.
    /// </summary>
    /// <typeparam name="T">Type for which a mock is needed to create</typeparam>
    internal interface IJournalMock<T>
    {
        T MockObject { get; }

        void Reset();

        bool VerifyFunctionCall(Expression<Action<T>> expression);

        dynamic GetMock();
    }

    /// <summary>
    /// Abstract the needed feature of mock here,
    /// If features more specific to MOQ or TELERIK is used then its useless,
    /// Use geneal features of a Mock
    /// </summary>
    internal class MOQMock<T> : IJournalMock<T> where T : class
    {
        private Mock<T> _mock = null;

        public MOQMock(Mock<T> mock)
        {
            this._mock = mock;
        }

        public T MockObject
        {
            get
            {
                return (T)_mock.Object;
            }
        }

        public dynamic GetMock()
        {
            return _mock;
        }

        public void Reset()
        {
            _mock.Reset();
        }

        public bool VerifyFunctionCall(Expression<Action<T>> expression)
        {
            try
            {
                this._mock.Verify(expression, Times.AtLeastOnce);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}