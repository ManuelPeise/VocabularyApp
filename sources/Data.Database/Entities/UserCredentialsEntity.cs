namespace Data.Database.Entities
{
    public class UserCredentialsEntity: AEntityBase
    {
        public string Salt { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
