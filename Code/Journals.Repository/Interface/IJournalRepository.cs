using Journals.Model;
using System.Collections.Generic;

namespace Journals.Repository
{
    public interface IJournalRepository
    {
        List<Journal> GetAllJournals();

        List<Journal> GetAllJournals(int userId);

        List<Journal> GetAllIssuesOfJournal(int journalId);

        Journal GetJournalById(int Id);

        OperationStatus AddJournal(Journal newJournal);

        OperationStatus DeleteJournal(Journal journal);

        OperationStatus UpdateJournal(Journal journal);

        OperationStatus AddJournalIssue(Journal newJournal);

        OperationStatus DeleteJournalIssue(Journal journal);
    }
}