using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static UniversityProject.Core.Entities.Auth.IdentityModel;

namespace UniversityProject.Infrastructure.Configuration.AuthConfiguration;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasData(new Role
        {
            Id = 1,
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR",

        });
    }
}
