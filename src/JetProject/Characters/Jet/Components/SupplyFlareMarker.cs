using JetProject.Characters.Jet.Systems;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// Marca projéteis da secundária (o sinalizador e a zona de fogo que ele cria) e liga cada um ao seu
    /// <see cref="SupplyShot"/>. Usado pelo <see cref="SupplyDropSystem"/> para atribuir mortes ao disparo.
    ///
    /// Repasse do disparo para a zona: <see cref="PendingShot"/> é definido imediatamente antes de disparar a zona;
    /// no servidor o spawn é síncrono, então o Awake da zona o consome. Sem pendência, o projétil inicia um disparo novo.
    /// </summary>
    public sealed class SupplyFlareMarker : MonoBehaviour
    {
        internal static SupplyShot PendingShot;

        /// <summary>True no prefab da zona de fogo (dano contínuo com chance de Burn).</summary>
        public bool IsFireZone;

        internal SupplyShot Shot { get; private set; }

        private void Awake()
        {
            if (!NetworkServer.active)
                return;
            Shot = PendingShot ?? new SupplyShot();
            PendingShot = null;
        }
    }
}
