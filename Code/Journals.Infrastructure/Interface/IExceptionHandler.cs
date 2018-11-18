namespace Journals.Infrastructure.Interface
{
    public interface IExceptionHandler
    {
        void HandleException(System.Exception ex);
    }
}