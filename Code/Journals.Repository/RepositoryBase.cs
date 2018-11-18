using Journals.Infrastructure.Interface;
using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Journals.Repository
{
    public class RepositoryBase<T> : IDisposable where T : DbContext, IDisposedTracker, new()
    {
        private IExceptionHandler _exceptionHandler;

        [Dependency]
        protected IExceptionHandler ExceptionHandler
        {
            get { return _exceptionHandler; }
            set { _exceptionHandler = value; }
        }

        private ICache _cache;

        [Dependency]
        protected ICache Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        public void SetExceptionHandler(IExceptionHandler exceptionHandler)
        {
            this._exceptionHandler = exceptionHandler;
        }

        private T _dataContext;

        public virtual T DataContext
        {
            get
            {
                if (_dataContext == null || _dataContext.IsDisposed)
                {
                    _dataContext = new T();

                    AllowSerialization = true;
                }
                return _dataContext;
            }
            protected set { _dataContext = value; }
        }

        protected virtual bool AllowSerialization
        {
            get
            {
                return _dataContext.Configuration.ProxyCreationEnabled;
            }
            set
            {
                _dataContext.Configuration.ProxyCreationEnabled = !value;
            }
        }

        protected virtual TEntity Get<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            TEntity entity = null;
            if (predicate != null)
            {
                try
                {
                    using (DataContext)
                    {
                        entity = DataContext.Set<TEntity>().Where(predicate).SingleOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            }
            else
            {
                throw new ApplicationException("Predicate value must be passed to Get<T>.");
            }

            return entity;
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            IQueryable<TEntity> queryableEntities = null;
            try
            {
                queryableEntities = DataContext.Set<TEntity>().AsQueryable();
                if (predicate != null)
                    queryableEntities = queryableEntities.Where(predicate);
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return queryableEntities;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DataContext != null)
                    DataContext.Dispose();
            }
        }
    }
}