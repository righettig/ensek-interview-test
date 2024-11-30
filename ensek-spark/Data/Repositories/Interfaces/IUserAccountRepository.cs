using ensek_spark.Models;

namespace ensek_spark.Data.Repositories.Interfaces;

public interface IUserAccountRepository
{
    Task<IEnumerable<UserAccount>> GetAllAsync();
    Task<UserAccount?> GetByIdAsync(string accountId);
}