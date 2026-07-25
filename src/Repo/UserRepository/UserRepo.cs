using Microsoft.EntityFrameworkCore;
using TodoAPI.DTOs;
using TodoAPI.Entities;

namespace TodoAPI.Repo.UserRepository
{
    public class UserRepo: IUserRepo
    {
        public UserRepo(TodoDbContext context) 
        { 
            _context = context;
        }

        private readonly TodoDbContext _context;

        public async Task<UserEntity> CreateAsync(UserEntity entity, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(UserEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            return entity;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            return entity;
        }

        public async Task<UserEntity?> UpdateAsync(UserEntity entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
