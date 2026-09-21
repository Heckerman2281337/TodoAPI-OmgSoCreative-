namespace TodoAPI.DTOs
{
    public record RegisterDTO(
        string Username, string Password,
        string ConfirmedPassword, string Email);
}
