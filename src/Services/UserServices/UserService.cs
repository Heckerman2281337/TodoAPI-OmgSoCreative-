using TodoAPI.Repo.UserRepository;
using TodoAPI.Entities;
using TodoAPI.DTOs;
using TodoAPI.Validators;
using Serilog;

namespace TodoAPI.Services.UserServices
{
    public class UserService(IUserRepo userRepository, IValidator<RegisterDTO> userValidator) : IUserService
    {
        private static readonly Serilog.ILogger _log = Log.ForContext<UserService>();

        public async Task CreateAsync(RegisterDTO dto, CancellationToken cancellationToken)
        {
            userValidator.Validate(dto);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var entity = new UserEntity(dto.Username, hashedPassword, dto.Email);
            await userRepository.CreateAsync(entity, cancellationToken);
            _log.Information("Пользователь {Username} успешно зарегистрирован с ID {UserId}", dto.Username, entity.UserId);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken); 
            if (entity == null)
            {
                _log.Warning("Попытка удаления несуществующего пользователя с ID {UserId}", id);
                throw new ArgumentException("Такого пользователя нет");
            }
            await userRepository.DeleteAsync(entity, cancellationToken);
            _log.Information("Пользователь {UserId} успешно удален", id);
        }

        public async Task<UserEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                _log.Warning("Запрошен несуществующий пользователь с ID {UserId}", id);
                throw new ArgumentException("Такого пользователя нет");
            }
            _log.Information("Данные пользователя {UserId} успешно запрошены", id);
            return entity;
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            var entity = await userRepository.GetByUsernameAsync(username, cancellationToken);
            if (entity == null)
            {
                _log.Warning("Запрошен несуществующий пользователь {Username}", username);
                throw new ArgumentException("Такого пользователя не существует");
            }
            _log.Information("Данные пользователя {Username} успешно запрошены", username);
            return entity;
        }

        public async Task UpdateAsync(UpdateUserDTO dto, Guid id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user is null)
            {
                _log.Warning("Попытка обновления несуществующего пользователя с ID {UserId}", id);
                throw new Exception("Такого пользователя не существует");
            }
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Update(dto.Username, hashedPassword);
            await userRepository.UpdateAsync(user, cancellationToken);
            _log.Information("Данные пользователя {UserId} успешно обновлены (Новый Username: {Username})", id, dto.Username);
        }
    }
}
