using Journals.Infrastructure.Interface;
using Journals.Model;
using Journals.Repository.DataContext;
using System.Linq;
using System.Web.Security;
using WebMatrix.WebData;

namespace Journals.Web
{
    public class StaticMembershipService : IStaticMembershipService
    {
        public dynamic GetUser()
        {
            var memershipUser = Membership.GetUser();
            return new UserProfile
            {
                UserId = (int)memershipUser.ProviderUserKey,
                UserName = memershipUser.UserName
            };
        }

        public void Initialize()
        {
            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("JournalsDB", "UserProfile", "UserId", "UserName", autoCreateTables: true);
        }

        public void Seed()
        {
            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if (!roles.RoleExists("Publisher"))
                roles.CreateRole("Publisher");

            if (!roles.RoleExists("Subscriber"))
                roles.CreateRole("Subscriber");

            using (var userContext = new UsersContext())
            {
                CreateAccountAndRole(roles, membership, userContext, "pappu", "Passw0rd", "pappu@journal.crossover.com", "Publisher");
                CreateAccountAndRole(roles, membership, userContext, "harold", "Passw0rd", "harold@journal.crossover.com", "Publisher");
                CreateAccountAndRole(roles, membership, userContext, "daniel", "Passw0rd", "daniel@journal.crossover.com", "Publisher");

                CreateAccountAndRole(roles, membership, userContext, "pappy", "Passw0rd", "pappy@journal.crossover.com", "Subscriber");
                CreateAccountAndRole(roles, membership, userContext, "andrew", "Passw0rd", "andrew@journal.crossover.com", "Subscriber");
                CreateAccountAndRole(roles, membership, userContext, "serge", "Passw0rd", "serge@journal.crossover.com", "Subscriber");

                userContext.SaveChanges();
            }
        }

        /// <summary>
        /// Refactor user creation and add email id to user profile
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="membership"></param>
        /// <param name="context"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        private void CreateAccountAndRole(SimpleRoleProvider roles, SimpleMembershipProvider membership, UsersContext context, string username, string password, string email, string role)
        {
            if (membership.GetUser(username, false) == null)
                membership.CreateUserAndAccount(username, password);

            if (!roles.GetRolesForUser(username).Contains(role))
                roles.AddUsersToRoles(new[] { username }, new[] { role });

            var user = context.UserProfiles.FirstOrDefault(u => u.UserName == username);
            if (user != null && string.IsNullOrEmpty(user.EmailId))
                user.EmailId = email;
        }

        public bool IsUserInRole(string userName, string roleName)
        {
            var roles = (SimpleRoleProvider)Roles.Provider;
            return roles.IsUserInRole(userName, roleName);
        }
    }
}