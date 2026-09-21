namespace TodoAPI.DTOs
{
    public record UpdateUserDTO(string Username, string Password,
        string ConfirmedPassword);
}
