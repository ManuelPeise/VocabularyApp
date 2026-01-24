namespace Shared.Models
{
    public class ApiResponseBase<TModel> where TModel : class
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public TModel? ResponseData { get; set; }
    }
}
