using Shared.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Database.Entities
{
    public class UserEntity: AEntityBase
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public int UserCredentialsId { get; set; }
        [ForeignKey(nameof(UserCredentialsId))]
        public UserCredentialsEntity UserCredentials { get; set; }
    }
}
