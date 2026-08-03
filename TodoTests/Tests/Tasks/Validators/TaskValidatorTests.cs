using FluentValidation.TestHelper;
using TodoAPI.DTOs;
using TodoAPI.Entities;
using TodoAPI.Validators;

namespace TodoTests.Tasks
{
    public class TaskValidatorTests
    {
        private readonly TaskValidator _validator = new();

        [Fact]
        public void Validate_DoesNotThrow_WhenTitleCorrect()
        {
            var dto = new TaskDTO(
                "My task",
                null,
                null,
                TaskCategory.None,
                TaskPriority.None);

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }


        [Fact]
        public void Validate_Throws_WhenTitleEmpty()
        {
            var dto = new TaskDTO(
                "",
                null,
                null,
                TaskCategory.None,
                TaskPriority.None);

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }


        [Fact]
        public void Validate_Throws_WhenTitleTooLong()
        {
            var dto = new TaskDTO(
                new string('a', 141),
                null,
                null,
                TaskCategory.None,
                TaskPriority.None);


            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }
    }
}