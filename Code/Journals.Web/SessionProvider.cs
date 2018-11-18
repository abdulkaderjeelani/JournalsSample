using System.Web;

namespace Jourals.Web
{
    public interface ISessionProvider
    {
        object this[string index]
        {
            get;
            set;
        }

        void Kill();
    }

    public class AspNetSessionProvider : ISessionProvider
    {
        public object this[string index]
        {
            get
            {
                return HttpContext.Current.Session[index];
            }
            set
            {
                HttpContext.Current.Session[index] = value;
            }
        }

        public void Kill()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }
    }
}