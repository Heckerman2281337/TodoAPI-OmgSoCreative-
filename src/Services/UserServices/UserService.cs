using TodoAPI.Repo.UserRepository;
using TodoAPI.Entities;
using TodoAPI.DTOs;
using BCrypt;
using BCrypt.Net;
using TodoAPI.Validators;

namespace TodoAPI.Services.UserServices
{
    public class UserService(IUserRepo userRepository, IValidator<RegisterDTO> userValidator) : IUserService
    {
        public async Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken)
        {
            userValidator.Validate(dto);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var entity = new UserEntity(dto.Username, hashedPassword, dto.Email);
            await userRepository.CreateAsync(entity, cancellationToken); 
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken); 
            if (entity == null)
                throw new ArgumentException("Такого пользователя нет");
            await userRepository.DeleteAsync(entity, cancellationToken);
        }

        public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
                throw new ArgumentException("Такого пользователя нет");
            return entity;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByUsernameAsync(username, cancellationToken);
            if (entity == null)
                throw new ArgumentException("Такого пользователя не существует");
            return entity;
        }

        public async Task UpdateAsync(UpdateUserDTO dto, Guid id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
                throw new Exception("Такого пользователя не существует");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            user.Update(dto.Username, hashedPassword);
            await userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
