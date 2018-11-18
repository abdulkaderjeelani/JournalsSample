using Journals.DigestMailService.Interface;
using Journals.Infrastructure;
using Journals.Infrastructure.Interface;
using Journals.Repository;
using Journals.Repository.Interface;
using Microsoft.Practices.Unity;
using System;

namespace Journals.DigestMailService
{
    internal class Program
    {
        private const string ExceptionHandlerProp = "ExceptionHandler";
        private static IUnityContainer _Instance = new UnityContainer();

        private static void Main(string[] args)
        {
            try
            {
                RegisterExternalDependancies();
                _Instance.RegisterType<DigestMailer>(new ContainerControlledLifetimeManager());
                var mailer = _Instance.Resolve<DigestMailer>();

#if DEBUG
                mailer.Run(DateTime.Now);
#else
                mailer.Run();
#endif
            }
            catch (Exception runEx)
            {
                //do event log / file log here
                Console.WriteLine("UN HANDLED EXCEPTION OCCURED " + runEx.Message);
            }
        }

        //NOTE:
        /*
         The registrations are spread out in both the web and external services,
         as the number of external services increase this will be a hazard boiler plate,
         to rectify, create a separate assembly for gneral unity  registration
         when there is 1 more service comes in
             */

        private static void RegisterExternalDependancies()
        {
            log4net.Config.XmlConfigurator.Configure();
            _Instance.RegisterType<IExceptionHandler, LogExceptionHandler>(new ContainerControlledLifetimeManager());
            _Instance.RegisterType<IMailer, Mailer>(new ContainerControlledLifetimeManager());
            _Instance.RegisterType<IMailServiceRepository, MailServiceRepository>(new HierarchicalLifetimeManager(), new InjectionProperty(ExceptionHandlerProp));
            _Instance.RegisterType<INotificationRepository, NotificationRepository>(new ContainerControlledLifetimeManager());
        }
    }
}