namespace Journals.Repository.DataContext
{
    /// <summary>
    /// Initialize this classs in application entry point, so that the context is operated, wiring
    /// up the codefirst to create db
    /// </summary>
    public class DbContextEnsure
    {
        public DbContextEnsure()
        {
            //operate on context to initialize db
            using (var journalsContext = new JournalsContext())
                journalsContext.Journals.Find(1);

            //operate on context to initialize db
            using (var usersContext = new UsersContext())
                usersContext.UserProfiles.Find(1);
        }
    }
}