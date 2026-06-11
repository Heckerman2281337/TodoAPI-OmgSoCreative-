using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoAPI.src.Entities;

namespace TodoAPI.src.Repo.UserRepository
{
    public class UserConfig : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(t => t.UserId);
            builder.Property(t => t.Username).IsRequired();
        }
    }
}
