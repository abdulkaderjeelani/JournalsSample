using System;

namespace Journals.Repository.DTO
{
    public class UserJournal
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int UserId { get; set; }
        public int? JournalId { get; set; }
        public bool IsSubscribed { get; set; }
    }
}