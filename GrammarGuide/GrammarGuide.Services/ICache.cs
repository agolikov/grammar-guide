using System.Threading.Tasks;

namespace GrammarGuide.Services;

public interface ICache
{
    Task<T> Get<T>(string key);
    Task<string> GetString(string key);
    Task Set<T>(string key, T value);
    Task Delete(string key);
}