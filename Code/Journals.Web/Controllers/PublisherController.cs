using AutoMapper;
using Jourals.Web.Controllers;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository;
using Journals.Web.Filters;
using Journals.Web.Helpers;
using Journals.Web.Model;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Journals.Web.Controllers
{
    [AuthorizeRedirect(Roles = "Publisher")]
    public class PublisherController : JournalDependantController
    {
        public PublisherController(IJournalRepository journalRepo, IStaticMembershipService membershipService)
            : base(membershipService, journalRepo)
        {
        }

        public ActionResult Index()
        {
            List<Journal> allJournals = _journalRepository.GetAllJournals(LoggedInUser.UserId);
            var journals = Mapper.Map<List<Journal>, List<JournalViewModel>>(allJournals);
            return View(journals);
        }

        public ActionResult Create()
        {
            ViewBag.CreateAction = "Create";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "JournalId")] JournalViewModel journal)
        {
            if (ModelState.IsValid)
            {
                var newJournal = Mapper.Map<JournalViewModel, Journal>(journal);
                //async use here
                JournalHelper.PopulateFile(journal.File, newJournal);

                newJournal.JournalId = null;
                newJournal.UserId = LoggedInUser.UserId;

                var opStatus = _journalRepository.AddJournal(newJournal);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));

                return RedirectToAction("Index");
            }
            else
                return View(journal);
        }

        public ActionResult Delete(int Id)
        {
            var selectedJournal = _journalRepository.GetJournalById(Id);
            var journal = Mapper.Map<Journal, JournalViewModel>(selectedJournal);
            if (journal.JournalId == 0)
                ViewBag.DeleteAction = "Delete";
            else
                ViewBag.DeleteAction = "DeleteIssue";
            return View(journal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(JournalViewModel journal)
        {
            var selectedJournal = Mapper.Map<JournalViewModel, Journal>(journal);

            var opStatus = _journalRepository.DeleteJournal(selectedJournal);
            if (!opStatus.Status)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            var journal = _journalRepository.GetJournalById(Id);

            var selectedJournal = Mapper.Map<Journal, JournalUpdateViewModel>(journal);

            return View(selectedJournal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(JournalUpdateViewModel journal)
        {
            if (ModelState.IsValid)
            {
                var selectedJournal = Mapper.Map<JournalUpdateViewModel, Journal>(journal);
                JournalHelper.PopulateFile(journal.File, selectedJournal);

                var opStatus = _journalRepository.UpdateJournal(selectedJournal);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

                return RedirectToAction("Index");
            }
            else
                return View(journal);
        }

        #region Journal Issues

        public ActionResult CreateIssue(int journalId)
        {
            ViewBag.JournalId = journalId;
            ViewBag.CreateAction = "CreateIssue";
            return View("Create");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateIssue(JournalViewModel journal)
        {
            if (ModelState.IsValid)
            {
                var newJournal = Mapper.Map<JournalViewModel, Journal>(journal);
                //async use here
                JournalHelper.PopulateFile(journal.File, newJournal);

                newJournal.UserId = LoggedInUser.UserId;

                var opStatus = _journalRepository.AddJournalIssue(newJournal);
                if (!opStatus.Status)
                    throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));

                return RedirectToAction("Issues", new { journalID = journal.JournalId });
            }
            else
                return RedirectToAction("CreateIssue", new { journalID = journal.JournalId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteIssue(JournalViewModel journal)
        {
            var selectedJournal = Mapper.Map<JournalViewModel, Journal>(journal);

            var opStatus = _journalRepository.DeleteJournalIssue(selectedJournal);
            if (!opStatus.Status)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            return RedirectToAction("Issues", new { journalID = journal.JournalId });
        }

        #endregion Journal Issues
    }
}