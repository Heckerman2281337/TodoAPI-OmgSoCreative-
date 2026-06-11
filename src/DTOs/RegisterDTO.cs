namespace TodoAPI.src.DTOs
{
    public class RegisterDTO
    {
        public RegisterDTO(string username, string password, string confirmedPassword, string email) 
        { 
            Username = username;
            Password = password;
            ConfirmedPassword = confirmedPassword;
            Email = email;
        }

        public string Username { get; init; }
        public string Password { get; init; }
        public string ConfirmedPassword {  get; init; }
        public string Email { get; init; }
    }
}
