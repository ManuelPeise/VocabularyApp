namespace Services.Shared.Constants
{
    /// <summary>
    /// API endpoint URLs for different deployment environments and platforms.
    /// </summary>
    public static class ApiConstants
    {
        /// <summary>
        /// API URL for Windows development (Visual Studio debug).
        /// </summary>
        public const string LocalhostUrl = "http://localhost:5000/api/";

        /// <summary>
        /// API URL for Windows development (Visual Studio debug).
        /// </summary>
        public const string DockerhostUrl = "http://192.168.178.46:5000/api/";
        /// <summary>
        /// Health check endpoint path.
        /// </summary>
        public const string HealthCheckEndpoint = "health/checkhealth";
    }
}
