using BepInEx.Logging;

namespace JetProject.Framework.Core
{
    /// <summary>Fachada estática de log. Use em qualquer módulo em vez de acessar o Logger do plugin.</summary>
    internal static class Log
    {
        private static ManualLogSource _source;

        public static void Init(ManualLogSource source) => _source = source;

        public static void Debug(object data) => _source?.LogDebug(data);
        public static void Info(object data) => _source?.LogInfo(data);
        public static void Warning(object data) => _source?.LogWarning(data);
        public static void Error(object data) => _source?.LogError(data);
    }
}
