using System.Text.RegularExpressions;
using TodoAPI.src.DTOs;

namespace TodoAPI.src.Validators
{
    public class UserValidator: IValidator<RegisterDTO>
    {
        //Regex patterns
        private static readonly Regex UsernameRegex = new(@"^[a-zA-Z0-9_]{3,20}$");
        private static readonly Regex EmailRegex = new(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$",RegexOptions.IgnoreCase);
        
        public void Validate(RegisterDTO user)
        {
            // Username validation
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Имя пользователя пустое");

            if (!UsernameRegex.IsMatch(user.Username))
                throw new ArgumentException("Имя пользователя должно содержать от 3 до 20 символов: буквы, цифры и _");

            //Password validation. Dont use regex, cause its too unreadable
            if (string.IsNullOrWhiteSpace(user.Password))
                throw new ArgumentException("Пароль пустой");

            if (user.Password.Length < 8)
                throw new ArgumentException("Пароль слишком короткий");

            if (!user.Password.Any(char.IsUpper))
                throw new ArgumentException("Нужна заглавная буква");

            if (!user.Password.Any(char.IsLower))
                throw new ArgumentException("Нужна строчная буква");

            if (!user.Password.Any(char.IsDigit))
                throw new ArgumentException("Нужна цифра");

            if (user.Password != user.ConfirmedPassword)
                throw new ArgumentException("Пароли не совпадают");

            //Email validation
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Нужна почта");

            if (!EmailRegex.IsMatch(user.Email))
                throw new ArgumentException("Некорректный email");
        }
    }
}
