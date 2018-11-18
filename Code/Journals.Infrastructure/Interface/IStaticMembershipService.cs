namespace Journals.Infrastructure.Interface
{
    public interface IStaticMembershipService
    {
        dynamic GetUser();

        bool IsUserInRole(string userName, string roleName);

        void Initialize();

        void Seed();
    }
}