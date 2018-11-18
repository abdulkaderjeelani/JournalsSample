using Journals.Infrastructure;
using Journals.Infrastructure.Interface;
using Journals.Repository;
using Journals.Services.ApplicationServices;
using Journals.Services.ApplicationServices.Interface;
using Journals.Web.Controllers;
using Microsoft.Practices.Unity;

namespace Journals.Web.IoC
{
    public static class IoCMappingContainer
    {
        private const string ExceptionHandlerProp = "ExceptionHandler";
        private const string CacherProp = "Cache";

        private static IUnityContainer _Instance = new UnityContainer();

        static IoCMappingContainer()
        {
        }

        public static IUnityContainer GetInstance()
        {
            //Register Infra services
            RegisterInfrastructureServices();

            //Register controllers
            RegisterControllers();

            //Register App services
            RegisterApplicationServices();

            //Register repositoryies
            RegisterRepositories();

            return _Instance;
        }

        private static void RegisterApplicationServices()
        {
            _Instance.RegisterType<ISubscriptionService, SubscriptionService>(new HierarchicalLifetimeManager(), new InjectionProperty(ExceptionHandlerProp));
        }

        private static void RegisterInfrastructureServices()
        {
            _Instance.RegisterType<IExceptionHandler, LogExceptionHandler>(new ContainerControlledLifetimeManager());
            _Instance.RegisterType<ICache, InMemoryCache>(new ContainerControlledLifetimeManager());
            _Instance.RegisterType<IStaticMembershipService, StaticMembershipService>(new HierarchicalLifetimeManager());
        }

        private static void RegisterRepositories()
        {
            _Instance.RegisterType<IJournalRepository, JournalRepository>(new HierarchicalLifetimeManager(), new InjectionProperty(ExceptionHandlerProp), new InjectionProperty(CacherProp));
            _Instance.RegisterType<ISubscriptionRepository, SubscriptionRepository>(new HierarchicalLifetimeManager(), new InjectionProperty(ExceptionHandlerProp), new InjectionProperty(CacherProp));
        }

        private static void RegisterControllers()
        {
            _Instance.RegisterType<HomeController>();
            _Instance.RegisterType<PublisherController>();
            _Instance.RegisterType<SubscriberController>();
        }
    }
}