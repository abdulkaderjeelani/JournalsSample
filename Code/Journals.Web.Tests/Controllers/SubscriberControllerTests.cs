using AutoMapper;
using Jourals.Web;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository.DTO;
using Journals.Web.Controllers;
using Journals.Web.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Journals.Web.Tests.Controllers
{
    [TestClass]
    public class SubscriberControllerTests
    {
        [TestMethod]
        public void Index_Returns_Journals_Flag_Set4_Subscribed()
        {
            //Arrange
            var profile = new UserProfile { UserId = 6 }; ;
            SubscriberController controller = this.ArrangeSubscriberController(profile);

            //Act
            var actionResult = controller.Index();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            var model = ((ViewResult)actionResult).Model as IEnumerable<UserJournal>;

            //moto to verify the flag is set correct only for the subscribed journals
            var subscribed = model.Where(m => m.IsSubscribed).Select(s => s.Id);
            if (subscribed != null && subscribed.Count() > 0)
            {
                var usersSubscribed = JournalMocks.SubscriptionData.Where(s => subscribed.Contains(s.JournalId)).Select(u => u.UserId);
                Assert.IsTrue(usersSubscribed.Contains(profile.UserId));
            }
        }

        private SubscriberController ArrangeSubscriberController(UserProfile profile)
        {
            Mapper.CreateMap<JournalViewModel, Journal>();
            Mapper.CreateMap<Journal, JournalViewModel>();

            var repository = JournalMocks.MockJournalRepositoryWithContext();
            var service = JournalMocks.MockSubscriptionServiceWithContext();
            IStaticMembershipService memServiceMock = JournalMocks.MockIStaticMembershipService(profile).MockObject;
            ISessionProvider sessionProvider = JournalMocks.MockISessionProvider(profile).MockObject;
            SubscriberController controller = new SubscriberController(repository, service, memServiceMock);
            controller.CheckRequestAuthenticatd = false;
            controller.SetSessionProvider(sessionProvider);
            return controller;
        }
    }
}