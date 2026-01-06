using Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enums;
using System.Globalization;

namespace Data.Database.Seeds
{
    internal class UserSeed : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            var timeStamp = DateTime.Parse("2026.01.05", CultureInfo.InvariantCulture);

            builder.HasData(new UserEntity
            {
                Id = 1,
                FirstName = "Admin",
                LastName = "User",
                DateOfBirth = DateTime.Parse("1980.04.20", CultureInfo.InvariantCulture),
                UserRole = UserRoleEnum.Admin,
                CreatedAt = timeStamp,
                CreatedBy = "System",
                UserCredentialsId = 1
            });
        }
    }
}
