using System.Linq;
using BepInEx.Configuration;

namespace JetProject.Framework.Core
{
    /// <summary>
    /// O BepInEx persiste os valores no .cfg, então mudar um valor padrão no código não afeta quem já tem o arquivo.
    /// Durante o balanceamento, incremente a revisão sempre que mudar padrões: na próxima execução, todas as
    /// entradas do arquivo voltam ao padrão do código (ajustes manuais no .cfg são descartados, de propósito).
    /// </summary>
    internal static class ConfigRevision
    {
        private const string Section = "_Meta";
        private const string Key = "DefaultsRevision";

        /// <summary>Chamar DEPOIS de todos os Bind do arquivo.</summary>
        public static void ResetIfOutdated(ConfigFile config, int currentRevision)
        {
            var stored = config.Bind(Section, Key, 0,
                "Controle interno: quando o mod muda os valores padrão, todas as opções voltam ao padrão. Não edite.");
            if (stored.Value == currentRevision)
                return;

            foreach (var entry in config.Keys.Select(k => config[k]).Where(e => e != stored))
                entry.BoxedValue = entry.DefaultValue;

            stored.Value = currentRevision;
            config.Save();
            Log.Info($"[Config] Padrões atualizados para a revisão {currentRevision}.");
        }
    }
}
