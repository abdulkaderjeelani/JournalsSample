using Journals.Infrastructure.Interface;
using System.Data.Entity;

namespace Journals.Repository.DataContext
{
    public class ModelChangedInitializer : DropCreateDatabaseIfModelChanges<JournalsContext>
    {
        protected readonly IStaticMembershipService _membershipService;

        public ModelChangedInitializer(IStaticMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        protected override void Seed(JournalsContext context)
        {
            DataInitializer.Initialize(context);
            _membershipService.Initialize();
            _membershipService.Seed();
            base.Seed(context);
        }
    }
}