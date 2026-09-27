namespace TodoAPI.Application.DTOs
{
    public record LoginResponseDTO(
        string RefreshToken, string AccessToken);
}
