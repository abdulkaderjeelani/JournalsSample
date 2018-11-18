using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Journals.Model
{
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        public string UserName { get; set; }

        /*ALTER TABLE [dbo].[UserProfile] ADD [EmailId] [varchar(max)]*/
        public string EmailId { get; set; }
    }
}