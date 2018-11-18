using AutoMapper;
using Jourals.Web;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository;
using Journals.Web.Controllers;
using Journals.Web.Model;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Journals.Web.Tests.Controllers
{
    [TestClass]
    public class PublisherControllerTest
    {
        [TestMethod]
        public void Index_Returns_All_Journals()
        {
            //Arrange
            IJournalRepository repository = null;
            PublisherController controller = ArrangePublisherController(out repository);

            //Act
            var actionResult = controller.Index();

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            var model = ((ViewResult)actionResult).Model as IEnumerable<JournalViewModel>;
            Assert.AreEqual(JournalMocks.JournalData.Count(c => c.JournalId == null || c.JournalId == 0), model.Count());
        }

        [TestMethod]
        public void Journal_Created_Redirect_Success()
        {
            //Arrange
            IJournalRepository repository = null;
            PublisherController controller = ArrangePublisherController(out repository);
            var file = JournalMocks.MockFile();

            //Act
            var actionResult = (RedirectToRouteResult)controller.Create(new JournalViewModel { Id = 1, Description = "TEST", File = file.MockObject, JournalId = null });

            //Assert
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
            Assert.AreEqual(((RedirectToRouteResult)actionResult).RouteValues["action"].ToString().Trim().ToUpper(), "INDEX");
        }

        private static PublisherController ArrangePublisherController(out IJournalRepository repository)
        {
            Mapper.CreateMap<JournalViewModel, Journal>();
            Mapper.CreateMap<Journal, JournalViewModel>();
            var profile = JournalMocks.StubUserProfile();
            repository = JournalMocks.MockJournalRepository(profile).MockObject;
            IStaticMembershipService memServiceMock = JournalMocks.MockIStaticMembershipService(profile).MockObject;
            ISessionProvider sessionProvider = JournalMocks.MockISessionProvider(profile).MockObject;
            PublisherController controller = new PublisherController(repository, memServiceMock);
            controller.CheckRequestAuthenticatd = false;
            controller.SetSessionProvider(sessionProvider);
            return controller;
        }
    }
}