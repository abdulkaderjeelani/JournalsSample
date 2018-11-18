namespace Journals.Model.DomainServices
{
    /// <summary>
    /// Contains the necessary domain service methods for our Journal application.
    /// </summary>
    internal class JournalService
    {
        /// <summary>
        /// Tells whether this is an issue of a journal or not,
        /// If this is true then only the JournalId makes sense,
        /// JournalId is the ID of the actual Journal this issue belongs to.
        /// </summary>
        internal static bool CheckWhetherJournalAnIssue(Journal journal)
        {
            return journal.JournalId > 0;
        }
    }
}