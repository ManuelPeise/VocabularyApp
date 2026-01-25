namespace Services.Shared.Models
{
    public class ChangePasswordResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
