using Journals.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Journals.Repository.DataContext
{
    public class JournalsContext : DbContext, IDisposedTracker
    {
        public JournalsContext()
            : base("name=JournalsDB")
        {
        }

        private bool _isDisposed = false;

        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.Configuration.LazyLoadingEnabled = false;
            modelBuilder.Entity<Journal>().ToTable("Journals");
            modelBuilder.Entity<Subscription>().ToTable("Subscriptions");
            base.OnModelCreating(modelBuilder);
        }

        protected override void Dispose(bool disposing)
        {
            //Make sure that the dispose is not called again on a disposed object
            if (IsDisposed) return;

            base.Dispose(disposing);
            _isDisposed = true;
        }
    }
}