using JetProject.Framework.Content;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Framework.Audio
{
    /// <summary>
    /// Sons disparados pelo SERVIDOR que precisam tocar em todos os clientes (ex.: algo que só o servidor decide).
    /// Sons disparados em código que já roda em todos os clientes (OnEnter de estados, Start de projéteis)
    /// devem usar Util.PlaySound direto.
    /// </summary>
    internal static class NetworkSounds
    {
        /// <summary>Cria e registra um NetworkSoundEventDef para um evento Wwise (vanilla ou do mod).</summary>
        public static NetworkSoundEventDef Create(ContentRegistry content, string eventName)
        {
            var def = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            ((ScriptableObject)def).name = "nse" + eventName;
            def.eventName = eventName;
            content.AddNetworkSound(def);
            return def;
        }

        /// <summary>Toca o som numa posição para todos os clientes. Só no servidor.</summary>
        public static void PlayAt(NetworkSoundEventDef def, Vector3 position)
        {
            if (def && NetworkServer.active)
                PointSoundManager.EmitSoundServer(def.index, position);
        }
    }
}
