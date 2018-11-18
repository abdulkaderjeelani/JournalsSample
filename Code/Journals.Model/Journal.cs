using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Journals.Model
{
    public class Journal
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public DateTime ModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public UserProfile User { get; set; }

        public int UserId { get; set; }

        /*ALTER TABLE [dbo].[Journals] ADD [JournalId] [int] NOT NULL DEFAULT 0*/

        /// <summary>
        /// If the value is 0, then it denotes this is a main journal, else
        /// this is an issue of the main journal with Id == JournalID
        /// </summary>
        public int? JournalId { get; set; }

        [ForeignKey("JournalId")]
        public Journal ParentJournal { get; set; }
    }
}