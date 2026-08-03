using FluentValidation.TestHelper;
using TodoAPI.DTOs;
using TodoAPI.Validators;

namespace TodoTests.User
{
    public class UserValidatorTests
    {
        private readonly UserValidator _validator = new();

        [Fact]
        public void Validate_DoesNotThrow_WhenUserIsValid()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Password123",
                ConfirmedPassword = "Password123",
                Email = "user@test.com"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }


        [Fact]
        public void Validate_Throws_WhenUsernameIsEmpty()
        {
            var dto = new RegisterDTO
            {
                Username = "",
                Password = "Password123",
                ConfirmedPassword = "Password123",
                Email = "user@test.com"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Username);
        }


        [Fact]
        public void Validate_Throws_WhenPasswordTooShort()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Pass1",
                ConfirmedPassword = "Pass1",
                Email = "user@test.com"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Password);
        }


        [Fact]
        public void Validate_Throws_WhenPasswordsDoNotMatch()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Password123",
                ConfirmedPassword = "Password321",
                Email = "user@test.com"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.ConfirmedPassword);
        }


        [Fact]
        public void Validate_Throws_WhenEmailInvalid()
        {
            var dto = new RegisterDTO
            {
                Username = "User123",
                Password = "Password123",
                ConfirmedPassword = "Password123",
                Email = "wrong-email"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }
    }
}