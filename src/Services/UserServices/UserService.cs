using TodoAPI.Repo.UserRepository;
using TodoAPI.Entities;
using TodoAPI.DTOs;
using FluentValidation;

namespace TodoAPI.Services.UserServices
{
    public class UserService(IUserRepo userRepository, 
        IValidator<RegisterDTO> userValidator, ILogger<UserService> logger) : IUserService
    {

        public async Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken)
        {
            var validation = await userValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var entity = new UserEntity(dto.Username, hashedPassword, dto.Email);
            await userRepository.CreateAsync(entity, cancellationToken);
            logger.LogInformation("Пользователь {Username} успешно зарегистрирован с ID {UserId}", dto.Username, entity.UserId);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken); 
            if (entity == null)
            {
                logger.LogWarning("Попытка удаления несуществующего пользователя с ID {UserId}", id);
                throw new ArgumentException("Такого пользователя нет");
            }
            await userRepository.DeleteAsync(entity, cancellationToken);
            logger.LogInformation("Пользователь {UserId} успешно удален", id);
        }

        public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Запрошен несуществующий пользователь с ID {UserId}", id);
                throw new ArgumentException("Такого пользователя нет");
            }
            logger.LogInformation("Данные пользователя {UserId} успешно запрошены", id);
            return entity;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByUsernameAsync(username, cancellationToken);
            if (entity == null)
            {
                logger.LogWarning("Запрошен несуществующий пользователь {Username}", username);
                throw new ArgumentException("Такого пользователя не существует");
            }
            logger.LogInformation("Данные пользователя {Username} успешно запрошены", username);
            return entity;
        }

        public async Task UpdateAsync(UpdateUserDTO dto, Guid id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                logger.LogInformation("Попытка обновления несуществующего пользователя с ID {UserId}", id);
                throw new KeyNotFoundException("Такого пользователя не существует");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Update(dto.Username, hashedPassword);
            await userRepository.UpdateAsync(user, cancellationToken);
            logger.LogInformation("Данные пользователя {UserId} успешно обновлены (Новый Username: {Username})", id, dto.Username);
        }
    }
}
