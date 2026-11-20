using System.IO;
using JetProject.Framework.Core;
using R2API;

namespace JetProject.Framework.Audio
{
    /// <summary>
    /// Registra no Wwise do jogo todos os soundbanks de Resources/SoundBanks (gerados por tools/audio/build-soundbank.ps1).
    /// Depois disso, qualquer evento do bank toca com o método nativo: <c>Util.PlaySound("Play_...", gameObject)</c>.
    /// </summary>
    internal sealed class AudioModule : IModule
    {
        public string Name => "Audio";

        public void Initialize(ModContext context)
        {
            var directory = Path.Combine(context.PluginDirectory, "Resources", "SoundBanks");
            if (!Directory.Exists(directory))
            {
                Log.Warning($"[Audio] Pasta de soundbanks ausente: {directory}");
                return;
            }

            foreach (var bank in Directory.GetFiles(directory, "*.bnk"))
            {
                SoundAPI.SoundBanks.Add(bank);
                Log.Info($"[Audio] Soundbank registrado: {Path.GetFileName(bank)}");
            }
        }
    }
}
