using Microsoft.EntityFrameworkCore;
using TodoAPI.Entities;

namespace TodoAPI.Repo.TokenRepository
{
    public class TokenRepo : ITokenRepo
    {
        public TokenRepo(TodoDbContext context)
        {
            _context = context;
        }

        private readonly TodoDbContext _context; 
        public async Task CreateAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            await _context.Tokens.AddAsync(refreshToken, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            _context.Tokens.Remove(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        
        public async Task DeleteExpiredAndRevokedAsync(CancellationToken cancellationToken)
        {
            await _context.Tokens.Where(t => t.ExpiresAt < DateTime.UtcNow || t.IsRevoked == true)
                .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<RefreshTokenEntity?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.TokenHash == refreshToken, cancellationToken);
        }

        public async Task RevokeTokenAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
        {
            _context.Tokens.Update(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
