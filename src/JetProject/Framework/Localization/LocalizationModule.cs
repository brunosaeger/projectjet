using System.IO;
using JetProject.Framework.Core;
using Language = RoR2.Language;
using Path = System.IO.Path;

namespace JetProject.Framework.Localization
{
    /// <summary>
    /// Registra Resources/Language como raiz de idiomas no sistema nativo do RoR2.
    /// Estrutura esperada: Resources/Language/&lt;codigo&gt;/*.json, com { "strings": { "TOKEN": "texto" } }.
    /// </summary>
    internal sealed class LocalizationModule : IModule
    {
        public string Name => "Localization";

        public void Initialize(ModContext context)
        {
            var languageRoot = Path.Combine(context.PluginDirectory, "Resources", "Language");
            if (!Directory.Exists(languageRoot))
            {
                Log.Warning($"[Localization] Pasta de idiomas ausente: {languageRoot}");
                return;
            }

            Language.collectLanguageRootFolders += folders => folders.Add(languageRoot);
        }
    }
}
