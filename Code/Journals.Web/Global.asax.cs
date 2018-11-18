using AutoMapper;
using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository.DataContext;
using Journals.Repository.DTO;
using Journals.Web.IoC;
using Journals.Web.Model;
using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Journals.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        private static DbContextEnsure _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        protected void Application_Start()
        {
            IUnityContainer mappingContainer = IoCMappingContainer.GetInstance();
            DependencyResolver.SetResolver(new IoCScopeContainer(mappingContainer));

            var membershipService = mappingContainer.Resolve<IStaticMembershipService>();
            Database.SetInitializer<JournalsContext>(new ModelChangedInitializer(membershipService));

            ConfigureMVC();

            RegisterAutoMappings();

            log4net.Config.XmlConfigurator.Configure();

            //Operate on context so that the model gets created
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            membershipService.Initialize(); //make sure the websecurity is initialized
        }

        private static void RegisterAutoMappings()
        {
            Mapper.CreateMap<Journal, UserJournal>();

            Mapper.CreateMap<Journal, JournalViewModel>();
            Mapper.CreateMap<JournalViewModel, Journal>();

            Mapper.CreateMap<Journal, JournalUpdateViewModel>();
            Mapper.CreateMap<JournalUpdateViewModel, Journal>();

            Mapper.CreateMap<Journal, UserJournal>();
            Mapper.CreateMap<UserJournal, Journal>();
        }

        private static void ConfigureMVC()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc.GetType() == typeof(HttpException))
            {
                if (exc.Message.Contains("Maximum request length exceeded."))
                    Response.Redirect(String.Format("~/Error/RequestLengthExceeded"));
            }
            else
            {
                Response.Redirect(String.Format("~/Error/Error"));
            }

            Server.ClearError();
        }
    }
}