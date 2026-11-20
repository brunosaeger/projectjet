using System.IO;
using BepInEx;
using JetProject.Characters;
using JetProject.Framework.Assets;
using JetProject.Framework.Audio;
using JetProject.Framework.Commands;
using JetProject.Framework.Content;
using JetProject.Framework.Core;
using JetProject.Framework.Localization;

namespace JetProject
{
    /// <summary>
    /// Composition root: o único lugar que conhece todos os módulos e define a ordem de inicialização.
    /// Não coloque lógica de jogo aqui, apenas a montagem dos módulos.
    /// </summary>
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("___riskofthunder.RoR2BepInExPack")]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    public sealed class JetPlugin : BaseUnityPlugin
    {
        private ModuleHost _host;

        private void Awake()
        {
            Log.Init(Logger);

            var context = new ModContext(Path.GetDirectoryName(Info.Location), Config);
            _host = new ModuleHost(context);

            // Framework (genérico): ordem importa, serviços primeiro.
            _host.Add(new AssetsModule());
            _host.Add(new AudioModule());
            _host.Add(new LocalizationModule());
            _host.Add(new ContentModule());
            _host.Add(new ChatCommandsModule());

            // Conteúdo (específico do mod): consome os serviços do framework.
            _host.Add(new CharactersModule());

            _host.InitializeAll();
        }
    }
}
