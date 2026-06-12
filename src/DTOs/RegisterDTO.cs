namespace TodoAPI.src.DTOs
{
    public class RegisterDTO
    {
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmedPassword {  get; init; }  = string.Empty;  
        public string Email { get; init; } = string.Empty;
    }
}
