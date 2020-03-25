using System;

namespace HADotNet.CommandCenter.Utils
{
    public static class SupervisorEnvironment
    {
        public const string SUPERVISOR_WEBSOCKET_URL = "http://supervisor/core/websocket";

        private const string TOKEN_ENV_NAME = "SUPERVISOR_TOKEN";
        private const string TOKEN_LEGACY_ENV_NAME = "HASSIO_TOKEN";

        public static string GetSupervisorToken() => Environment.GetEnvironmentVariable(TOKEN_ENV_NAME) ?? Environment.GetEnvironmentVariable(TOKEN_LEGACY_ENV_NAME);

        public static string GetBaseUrl() => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(TOKEN_ENV_NAME))
            ? "http://supervisor"
            : "http://hassio";
    }
}
