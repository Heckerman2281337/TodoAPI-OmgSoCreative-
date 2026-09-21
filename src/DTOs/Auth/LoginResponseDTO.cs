namespace TodoAPI.DTOs
{
    public record LoginResponseDTO(
        string RefreshToken, string AccessToken);
}
