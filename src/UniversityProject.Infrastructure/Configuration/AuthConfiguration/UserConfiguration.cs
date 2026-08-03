using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static UniversityProject.Core.Entities.Auth.IdentityModel;

namespace UniversityProject.Infrastructure.Configuration.AuthConfiguration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        var hasher = new PasswordHasher<User>();
        builder.HasData(new User
        {
            Id = 1,
            Email = "admin@localhost.com",
            NormalizedEmail = "ADMIN@LOCALHOST.COM",
            UserName = "admin@localhost.com",
            NormalizedUserName = "ADMIN@LOCALHOST.COM",
            PasswordHash = hasher.HashPassword(null, "P@ssword1"),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });
    }
}
