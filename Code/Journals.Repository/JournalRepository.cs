using Journals.Model;
using Journals.Repository.DataContext;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;

namespace Journals.Repository
{
    public class JournalRepository : RepositoryBase<JournalsContext>, IJournalRepository
    {
        public JournalRepository()
        {
        }

        public JournalRepository(JournalsContext context)
        {
            this.DataContext = context;
        }

        public List<Journal> GetAllJournals()
        {
            List<Journal> list = null;
            try
            {
                list = GetJournalCache();
                if (list == null)
                {
                    using (this)
                        list = this.GetQueryable<Journal>(j => j.Id > 0 && (j.JournalId == 0 || j.JournalId == null)).ToList();

                    PutJournalCache(list);
                }
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }

            return list;
        }

        public List<Journal> GetAllJournals(int userId)
        {
            List<Journal> list = null;
            try
            {
                using (this)
                    list = this.GetQueryable<Journal>(j => j.Id > 0 && j.UserId == userId && (j.JournalId == 0 || j.JournalId == null)).ToList();
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }

            return list;
        }

        public List<Journal> GetAllIssuesOfJournal(int journalId)
        {
            List<Journal> list = null;
            try
            {
                using (DataContext)
                    list = this.GetQueryable<Journal>(j => j.JournalId == journalId).ToList();
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }

            return list;
        }

        public Journal GetJournalById(int Id)
        {
            Journal journal = null;
            try
            {
                using (this)
                    journal = this.Get<Journal>(j => j.Id == Id);
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }

            return journal;
        }

        public OperationStatus AddJournal(Journal newJournal)
        {
            var opStatus = new OperationStatus { Status = true };
            try
            {
                using (DataContext)
                {
                    newJournal.ModifiedDate = DateTime.Now;
                    var j = DataContext.Journals.Add(newJournal);
                    DataContext.SaveChanges();
                    opStatus.Id = j.Id;
                    opStatus.RecordsAffected++;
                    RemoveJournalCache();
                }
            }
            catch (Exception e)
            {
                opStatus.CreateFromException("Error adding journal: ", e);
                ExceptionHandler.HandleException(e);
            }

            return opStatus;
        }

        public OperationStatus DeleteJournal(Journal journal)
        {
            var opStatus = new OperationStatus { Status = true };
            try
            {
                using (DataContext)
                {
                    var subscriptions = DataContext.Subscriptions.Where(j => j.JournalId == journal.Id);
                    foreach (var subscription in subscriptions)
                    {
                        DataContext.Subscriptions.Remove(subscription);
                        opStatus.RecordsAffected++;
                    }

                    var journalToDelete = DataContext.Journals.Find(journal.Id);
                    DataContext.Journals.Remove(journalToDelete);
                    DataContext.SaveChanges();
                    RemoveJournalCache();
                }
            }
            catch (Exception e)
            {
                opStatus.CreateFromException("Error deleting journal: ", e);
                ExceptionHandler.HandleException(e);
            }

            return opStatus;
        }

        public OperationStatus UpdateJournal(Journal journal)
        {
            var opStatus = new OperationStatus { Status = true };
            try
            {
                var j = DataContext.Journals.Find(journal.Id);
                if (journal.Title != null)
                    j.Title = journal.Title;

                if (journal.Description != null)
                    j.Description = journal.Description;

                if (journal.Content != null)
                    j.Content = journal.Content;

                if (journal.ContentType != null)
                    j.ContentType = journal.ContentType;

                if (journal.FileName != null)
                    j.FileName = journal.FileName;

                j.ModifiedDate = DateTime.Now;

                DataContext.Entry(j).State = EntityState.Modified;
                DataContext.SaveChanges();
                opStatus.RecordsAffected++;
                RemoveJournalCache();
            }
            catch (DbEntityValidationException e)
            {
                string error = string.Empty;
                foreach (var eve in e.EntityValidationErrors)
                {
                    error += string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State) + Environment.NewLine;
                }

                opStatus.CreateFromException(error, e);
                ExceptionHandler.HandleException(e);
            }
            catch (Exception e)
            {
                opStatus.CreateFromException("Error updating journal: ", e);
                ExceptionHandler.HandleException(e);
            }

            return opStatus;
        }

        /// <summary>
        /// Internally we are using the journal to store the issue also but abstract this from the caller
        /// </summary>
        /// <param name="newJournal"></param>
        /// <returns></returns>
        public OperationStatus AddJournalIssue(Journal newJournal)
        {
            return AddJournal(newJournal);
        }

        public OperationStatus DeleteJournalIssue(Journal journal)
        {
            return DeleteJournal(journal);
        }

        #region Cache

        private const string JournalCacheKey = "__AllJournals";

        private List<Journal> GetJournalCache()
        {
            return Cache != null ? Cache.Get<List<Journal>>(JournalCacheKey) : null;
        }

        private void PutJournalCache(List<Journal> list)
        {
            if (Cache != null)
                Cache.Put<List<Journal>>(JournalCacheKey, list);
        }

        private void RemoveJournalCache()
        {
            if (Cache != null)
                Cache.Remove(JournalCacheKey);
        }

        #endregion Cache
    }
}