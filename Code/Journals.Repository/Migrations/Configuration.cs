namespace Medico.Repository.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Journals.Repository.DataContext.JournalsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Journals.Repository.DataContext.JournalsContext context)
        {
        }
    }
}