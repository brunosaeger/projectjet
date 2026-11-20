using RoR2;
using UnityEngine;

namespace JetProject.Framework.Audio
{
    /// <summary>
    /// Toca um evento Wwise num emissor próprio e temporário, independente de quem disparou.
    /// O Wwise corta sons quando o GameObject que os emite é destruído (ex.: um projétil no impacto); aqui o emissor
    /// vive <c>lifetime</c> segundos, garantindo que o som toque até o fim. Roda localmente (chamar em todos os clientes).
    /// </summary>
    internal static class DetachedSound
    {
        /// <param name="layers">Quantas vezes postar o evento junto. 2 ≈ +6 dB (forma de aumentar o volume de um som vanilla).</param>
        public static void Play(string eventName, Vector3 position, float lifetime = 6f, int layers = 1)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

            var emitter = new GameObject("JetProject_Sound_" + eventName);
            emitter.transform.position = position;
            for (int i = 0; i < Mathf.Max(1, layers); i++)
                Util.PlaySound(eventName, emitter);
            Object.Destroy(emitter, lifetime);
        }
    }
}
