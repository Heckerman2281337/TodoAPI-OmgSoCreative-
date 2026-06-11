namespace TodoAPI.src.DTOs
{
    public class LoginDTO
    {
        public LoginDTO(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; init; }
        public string Password { get; init; }
    }
}
