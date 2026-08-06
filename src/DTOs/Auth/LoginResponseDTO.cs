namespace TodoAPI.DTOs
{
    public class LoginResponseDTO
    {
        public LoginResponseDTO(string refreshToken, string accessToken) 
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; private set; }
        public string RefreshToken { get; private set; }

    }
}
