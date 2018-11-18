using AutoMapper;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository;
using Journals.Web.Model;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace Jourals.Web.Controllers
{
    public class ControllerBase : System.Web.Mvc.Controller
    {
        protected ISessionProvider _sessionProvider = null;

        public ControllerBase()
        {
            _sessionProvider = new AspNetSessionProvider();//USE IOC if needed, dont complicae things if not needed
        }

        private IExceptionHandler _exceptionHandler;

        [Dependency]
        protected IExceptionHandler ExceptionHandler
        {
            get { return _exceptionHandler; }
            set { _exceptionHandler = value; }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            ExceptionHandler.HandleException(filterContext.Exception);
            base.OnException(filterContext);
        }
    }

    public class ControllerForAuthenticated : ControllerBase
    {
        protected readonly IStaticMembershipService _membershipService;
        public bool CheckRequestAuthenticatd { get; set; }

        public ControllerForAuthenticated(IStaticMembershipService membershipService)
        {
            this._membershipService = membershipService;
            CheckRequestAuthenticatd = true;
        }

        public ControllerForAuthenticated(IStaticMembershipService membershipService, ISessionProvider sessionProvider)
            : this(membershipService)
        {
            this._sessionProvider = sessionProvider;
        }

        public void SetSessionProvider(ISessionProvider sessionProvider)
        {
            this._sessionProvider = sessionProvider;
        }

        //This is to ensure the deserilization from session does not happen everytime when requested, once per request
        private UserProfile _loggedInUser = null;

        /*do not use session as it is used here, use a manager for session and abstract it*/

        public UserProfile LoggedInUser
        {
            get
            {
                if (CheckRequestAuthenticatd && !Request.IsAuthenticated) return null;

                if (_sessionProvider["LoggedInUser"] == null)
                    _sessionProvider["LoggedInUser"] = _membershipService.GetUser();

                if (_loggedInUser == null)
                    _loggedInUser = _sessionProvider["LoggedInUser"] as UserProfile;

                return _loggedInUser;
            }
        }
    }

    public class JournalDependantController : ControllerForAuthenticated
    {
        protected readonly IJournalRepository _journalRepository;

        public JournalDependantController(IStaticMembershipService membershipService, IJournalRepository journalRepo)
            : base(membershipService)
        {
            _journalRepository = journalRepo;
        }

        public ActionResult GetFile(int Id)
        {
            Journal j = _journalRepository.GetJournalById(Id);
            if (j == null)
                throw new System.Web.Http.HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));

            if (j.Content != null && j.ContentType != null)
                return File(j.Content, j.ContentType);
            else
                return RedirectToAction("Error", "Error");
        }

        public ActionResult Issues(int journalID)
        {
            ViewBag.JournalId = journalID;
            List<Journal> allJournals = _journalRepository.GetAllIssuesOfJournal(journalID);
            var journals = Mapper.Map<List<Journal>, List<JournalViewModel>>(allJournals);
            return View(journals);
        }
    }
}