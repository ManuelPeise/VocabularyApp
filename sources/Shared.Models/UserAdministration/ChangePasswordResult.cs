namespace Shared.Models.UserAdministration
{
    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
