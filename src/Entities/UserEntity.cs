namespace TodoAPI.src.Entities
{
    public class UserEntity
    {
        public UserEntity(string username, string hashedPassword, string email) 
        { 
            Username = username;
            HashedPassword = hashedPassword;
            Email = email;
            UserId = Guid.NewGuid();
            UserCreated = DateTime.UtcNow;
        }

        public string Username { get; private set; }
        public string HashedPassword { get; private set; }
        public string Email { get; private set; }
        public DateTime UserCreated { get; private set; }
        public Guid UserId { get; private set; }
        public List<TaskEntity> Tasks { get; private set; } = new List<TaskEntity>();

        public void Update(string username, string password)
        {
            Username = username;
            HashedPassword = password;
        }

    }

}
