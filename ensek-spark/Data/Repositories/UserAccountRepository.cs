using ensek_spark.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_spark.Data.Repositories;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly UserAccountContext _context;

    public UserAccountRepository(UserAccountContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<UserAccount>> GetAllAsync()
    {
        return await _context.Set<UserAccount>().ToListAsync();
    }

    public async Task<UserAccount?> GetByIdAsync(string accountId)
    {
        return await _context.Set<UserAccount>().FirstOrDefaultAsync(u => u.AccountId == accountId);
    }
}

