using TodoAPI.Entities;

namespace TodoTests.User
{
    public class UserEntityTests
    {
        // Constructor tests
        [Fact]
        public void Constructor_SetsAllPropertiesCorrectly()
        {
            var username = "TestUser";
            var password = "hashedPassword";
            var email = "test@test.com";

            var user = new UserEntity(username, password, email);

            Assert.Equal(username, user.Username);
            Assert.Equal(password, user.HashedPassword);
            Assert.Equal(email, user.Email);
            Assert.NotEqual(Guid.Empty, user.UserId);
            Assert.Empty(user.Tasks);
        }


        [Fact]
        public void Constructor_SetsUserCreatedToUtcNow()
        {
            var before = DateTime.UtcNow;
            var user = new UserEntity("TestUser", "hashedPassword", "test@test.com");
            var after = DateTime.UtcNow;


            Assert.InRange(user.UserCreated, before, after);
        }

        // Update tests
        [Fact]
        public void Update_ChangesUsernameAndPassword()
        {
            var user = new UserEntity("OldUsername", "OldPasswordHash", "test@test.com");

            user.Update("NewUsername", "NewPasswordHash");

            Assert.Equal(
                "NewUsername",
                user.Username);
            Assert.Equal(
                "NewPasswordHash",
                user.HashedPassword);
        }

        [Fact]
        public void Update_DoesNotChangeEmailOrId()
        {
            var user = new UserEntity("OldUsername", "OldPasswordHash", "test@test.com");
            var oldId = user.UserId;
            var oldEmail = user.Email;

            user.Update(
                "NewUsername",
                "NewPasswordHash");


            Assert.Equal(
                oldId,
                user.UserId);
            Assert.Equal(
                oldEmail,
                user.Email);
        }
    }
}