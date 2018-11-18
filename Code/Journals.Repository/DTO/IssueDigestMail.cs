using System;

namespace Journals.Repository.DTO
{
    /// <summary>
    /// DTO for mail program,
    /// </summary>
    public class IssueDigestMail
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string EmailId { get; set; }

        public int JournalId { get; set; }
        public string JournalTitle { get; set; }

        public string IssueTitle { get; set; }
        public DateTime IssueDate { get; set; }
    }
}