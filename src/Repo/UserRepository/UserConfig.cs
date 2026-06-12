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
            builder.Property(t => t.Username).IsRequired().HasMaxLength(20);
            builder.Property(t => t.HashedPassword).IsRequired();
            builder.Property(t => t.Email).IsRequired();

            builder.HasIndex(t => t.Username).IsUnique();
            builder.HasIndex(t => t.Email).IsUnique();
        }
    }
}
