using System.Text.RegularExpressions;
using TodoAPI.DTOs;
using FluentValidation;

namespace TodoAPI.Validators
{
    public class UserValidator: AbstractValidator<RegisterDTO>
    {
        public UserValidator()
        {
            // Username validation
            RuleFor(user => user.Username)
                .NotEmpty().WithMessage("Имя пользователя пустое")
                .Length(3, 20).WithMessage("Имя пользователя должно содержать от 3 до 20 символов")
                .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Имя пользователя может содержать только буквы, цифры и знак подчеркивания");

            // Password validation
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Пароль не может быть пустым")
                .MinimumLength(8).WithMessage("Пароль слишком короткий (минимум 8 символов)")
                .Matches(@"[A-Z]+").WithMessage("Пароль должен иметь хотя бы 1 заглавную букву")
                .Matches(@"[a-z]+").WithMessage("Пароль должен иметь хотя бы 1 строчную букву")
                .Matches(@"[0-9]+").WithMessage("Пароль должен иметь хотя бы 1 цифру")
                .Must(p => !p.Contains(" ")).WithMessage("Пароль не должен содержать пробелы");

            RuleFor(user => user.ConfirmedPassword)
                .NotEmpty().WithMessage("Подтверждение пароля не может быть пустым")
                .Equal(user => user.Password).WithMessage("Пароли не совпадают");

            //Email validation
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Электронная почта пустая")
                .EmailAddress().WithMessage("Некорректный email");
        }       
    }
}
