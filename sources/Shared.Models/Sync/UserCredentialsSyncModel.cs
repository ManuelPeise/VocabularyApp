namespace Shared.Models.Sync
{
    public class UserCredentialsSyncModel : SyncModelBase
    {
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
