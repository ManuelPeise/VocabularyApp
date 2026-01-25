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
        public const string LocalhostUrl = "http://localhost:5218/api/";

        /// <summary>
        /// API URL for Android emulator (10.0.2.2 maps to host machine).
        /// </summary>
        public const string AndroidEmulatorUrl = "http://10.0.2.2:5218/api/";

        /// <summary>
        /// API URL for Android physical devices on local network.
        /// </summary>
        public const string LocalNetworkUrl = "http://192.168.178.46:5218/api/";

        /// <summary>
        /// Health check endpoint path.
        /// </summary>
        public const string HealthCheckEndpoint = "health/checkhealth";
    }
}
