using Jourals.Web.Controllers;
using Journals.Infrastructure.Interface;
using Journals.Repository;
using Journals.Repository.DTO;
using Journals.Services.ApplicationServices.Interface;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Journals.Web.Controllers
{
    [Authorize]
    public class SubscriberController : JournalDependantController
    {
        private ISubscriptionService _subscriptionService = null;

        public SubscriberController(IJournalRepository journalRepo, ISubscriptionService subscriptionService, IStaticMembershipService membershipService)
            : base(membershipService, journalRepo)
        {
            _subscriptionService = subscriptionService;
        }

        public ActionResult Index(string errorMessage = null)
        {
            List<UserJournal> userJournals = _subscriptionService.GetUserJournals(LoggedInUser.UserId);

            if (userJournals == null || userJournals.Count == 0)
                errorMessage = (errorMessage ?? string.Empty) + "No journals are available for subscription. " + (ViewBag.ErrorMessage ?? string.Empty);

            ViewBag.ErrorMessage = errorMessage;
            return View(userJournals);
        }

        public ActionResult Subscribe(int Id)
        {
            string errorMessage = null;
            var opStatus = _subscriptionService.Subscribe(Id, LoggedInUser.UserId);

            if (!opStatus)
                errorMessage = "Sorry, we could not process your subscription request now. Please try again later.";

            return RedirectToAction("Index", errorMessage);
        }

        public ActionResult UnSubscribe(int Id)
        {
            string errorMessage = null;
            var opStatus = _subscriptionService.UnSubscribe(Id, LoggedInUser.UserId);

            if (!opStatus)
                errorMessage = "Sorry, we could not process your un subscription request now. Please try again later.";

            return RedirectToAction("Index", errorMessage);
        }
    }
}