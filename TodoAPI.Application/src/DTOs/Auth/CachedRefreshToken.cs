namespace TodoAPI.Application.src.DTOs.Auth
{
    public record CachedRefreshToken(Guid Id, Guid UserId, bool IsRevoked, DateTime ExpiresAt);

}
