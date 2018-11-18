using Journals.Services.ApplicationServices.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Journals.Web.Tests.Services
{
    [TestClass]
    public class SubscriptionServiceTest
    {
        private ISubscriptionService service = null;

        [TestInitialize]
        public void TestInit()
        {
            service = JournalMocks.MockSubscriptionServiceWithContext();
        }

        [TestMethod]
        public void GetUserJournals_Retrieves_Only_Of_User()
        {
            const int userId = 1;

            //Act
            var userJournals = service.GetUserJournals(userId);

            //Assert
            Assert.IsTrue(userJournals.All(s => s.UserId == userId));
        }

        [TestMethod]
        public void Subscribe_Adds_To_List()
        {
            //Act - provided journal id 1 already exist in my stub

            const int journalId = 1;
            const int userId = 3;

            service.Subscribe(journalId, userId);

            //Assert
            var userJournals = service.GetUserJournals(userId);
            Assert.IsTrue(userJournals.Any(s => s.Id == journalId && s.IsSubscribed == true));
        }

        [TestMethod]
        public void UnSubscribe_Removes_From_List()
        {
            const int journalId = 1;
            const int userId = 3;

            //Act - provided journal id 1 already exist in my stub and subscription for user 1 already exist
            service.UnSubscribe(journalId, userId);

            //Assert
            var userJournals = service.GetUserJournals(userId);
            Assert.IsTrue(userJournals.Any(s => s.Id == journalId && s.IsSubscribed == false));
        }
    }
}