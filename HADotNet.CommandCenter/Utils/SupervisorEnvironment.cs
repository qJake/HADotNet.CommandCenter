using System;

namespace HADotNet.CommandCenter.Utils
{
    public static class SupervisorEnvironment
    {
        public const string SUPERVISOR_WEBSOCKET_URL = "http://supervisor/core/websocket";

        private const string TOKEN_ENV_NAME = "SUPERVISOR_TOKEN";
        private const string TOKEN_LEGACY_ENV_NAME = "HASSIO_TOKEN";

        public static bool IsNewSupervisor => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(TOKEN_ENV_NAME));

        /// <summary>
        /// Gets if we think we are running in a Supervisor add-on environment or not.
        /// </summary>
        public static bool IsSupervisorAddon => !string.IsNullOrWhiteSpace(GetSupervisorToken());

        /// <summary>
        /// Gets the supervisor auth token via the environment variable.
        /// </summary>
        public static string GetSupervisorToken() => Environment.GetEnvironmentVariable(TOKEN_ENV_NAME) ?? Environment.GetEnvironmentVariable(TOKEN_LEGACY_ENV_NAME);

        /// <summary>
        /// Gets the base API URL for supervisor API interaction.
        /// </summary>
        public static string GetBaseUrl() => IsNewSupervisor ? "http://supervisor/core" : "http://hassio/core";
    }
}
