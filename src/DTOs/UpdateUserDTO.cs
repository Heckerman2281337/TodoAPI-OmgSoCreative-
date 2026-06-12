namespace TodoAPI.src.DTOs
{
    public class UpdateUserDTO
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmedPassword { get; init; } = string.Empty;
    }
}
