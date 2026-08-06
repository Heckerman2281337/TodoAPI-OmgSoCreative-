using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoAPI.Entities;

namespace TodoAPI.Repo.TokenRepository
{
    public class TokenConfig : IEntityTypeConfiguration<RefreshTokenEntity>
    {
        public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.TokenHash).IsUnique();
            builder.Property(t => t.TokenHash).IsRequired();
            builder.Property(t => t.ExpiresAt).IsRequired();
            
            builder.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
