using TodoAPI.src.DTOs;

namespace TodoAPI.src.Validators
{
    public interface IValidator<T>
    {
        void Validate(T value);
    }

    public class TaskValidator: IValidator<TaskDTO>
    {
        public void Validate(TaskDTO task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("У задачи должно быть название");

            if (task.Title.Length > 140)
                throw new ArgumentException("Название задачи не должно превышать 140 символов");
        }
    }

    public class UpdatedTaskValidator: IValidator<UpdateTaskDTO>
    {
        public void Validate(UpdateTaskDTO task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("У задачи должно быть название");

            if (task.Title.Length > 140)
                throw new ArgumentException("Название задачи не должно превышать 140 символов");
        }
    }
}
