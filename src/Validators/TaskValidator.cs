using FluentValidation;
using TodoAPI.DTOs;

namespace TodoAPI.Validators
{
    public class TaskValidator : AbstractValidator<TaskDTO>
    {
        public TaskValidator()
        {
            RuleFor(task => task.Title)
                .NotEmpty().WithMessage("У задачи должно быть название")
                .MaximumLength(140).WithMessage("Название задачи не должно превышать 140 символов");

            RuleFor(task => task.Deadline)
                .Must(date => date is null || date != default(DateTime))
                .WithMessage("Неккоректная дата");
        }
    }

    //Same thing as TaskValidator - fix later
    public class UpdatedTaskValidator : AbstractValidator<UpdateTaskDTO>
    {
        public UpdatedTaskValidator()
        {
            RuleFor(task => task.Title)
                .NotEmpty().WithMessage("У задачи должно быть название")
                .MaximumLength(140).WithMessage("Название задачи не должно превышать 140 символов");

            RuleFor(task => task.Deadline)
                .Must(date => date is null || date != default(DateTime))
                .WithMessage("Неккоректная дата");
        }
    }
}
