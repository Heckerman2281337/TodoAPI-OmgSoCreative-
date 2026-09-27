namespace TodoAPI.Application.DTOs
{
    public record RegisterDTO(
        string Username, string Password,
        string ConfirmedPassword, string Email);
}
