using TodoAPI.src.Repo.UserRepository;
using TodoAPI.src.Entities;
using TodoAPI.src.DTOs;
using BCrypt;
using BCrypt.Net;

namespace TodoAPI.src.Services.UserServices
{
    public class UserService(IUserRepo _userRepository) : IUserService
    {
        public async Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken)
        {
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
           
            var user = new UserEntity(dto.Username, hashedPassword, dto.Email);

            await _userRepository.CreateAsync(user);
            
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            
        }

        public Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(LoginDTO dto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
