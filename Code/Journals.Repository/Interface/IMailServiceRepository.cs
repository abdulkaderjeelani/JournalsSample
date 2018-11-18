using System.Collections.Generic;

namespace Journals.Repository.Interface
{
    /*
     NOTE: If we give access to journal repository for mail program, then there is a possiblity of
     calling unintended functions like add journal, so we create a separate interface, this also complements to
     ISP, Journal Repository does not mean to implement this function by its nature.*/

    /// <summary>
    /// Interface to be called by external mail program,
    /// </summary>
    public interface IMailServiceRepository
    {
        /// <summary>
        /// Gets the issues of journals user subscribed that posted on a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        List<DTO.IssueDigestMail> GetIssuesForDigestMail(System.DateTime date);
    }
}