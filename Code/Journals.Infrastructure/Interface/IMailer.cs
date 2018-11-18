namespace Journals.Infrastructure.Interface
{
    public interface IMailer
    {
        /// <summary>
        /// sends out emails, if there is any failure in sending email its the responsibily of this function to take care of.
        /// The caller believes once this function is called then the mails are sent.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        void SendMail(string from, string to, string subject, string body);
    }
}