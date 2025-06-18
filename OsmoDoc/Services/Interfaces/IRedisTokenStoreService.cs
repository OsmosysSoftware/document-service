using System.Threading.Tasks;

namespace OsmoDoc.Services;

public interface IRedisTokenStoreService
{
    Task StoreTokenAsync(string token, string email);
    Task<bool> IsTokenValidAsync(string token);
    Task RevokeTokenAsync(string token);
}