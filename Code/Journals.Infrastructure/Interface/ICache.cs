namespace Journals.Infrastructure.Interface
{
    public interface ICache
    {
        void Put<T>(string key, T toCache);

        T Get<T>(string key);

        void Remove(string key);

        void Clear();
    }
}