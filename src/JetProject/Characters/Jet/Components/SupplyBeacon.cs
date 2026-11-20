using JetProject.Framework.Audio;
using JetProject.Framework.Interactables;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// Marcador de entrega (feixe da utilitária do Captain) no chão onde o inimigo morreu.
    /// O feixe SEMPRE aparece (com o som de aviso do Captain). Se o sorteio passou, no fim do feixe:
    /// impacto do Orbital Probe (poeira, anéis, tremor de tela, som de impacto) + som de queda da entrega
    /// + baú EXATAMENTE no ponto do marcador.
    ///
    /// O payload é passado por <see cref="PendingPayload"/>: o servidor cria o projétil de forma síncrona dentro de
    /// ProjectileManager.FireProjectile, então o Awake deste componente roda dentro dessa chamada e o consome.
    /// </summary>
    public sealed class SupplyBeacon : MonoBehaviour
    {
        /// <summary>Definido imediatamente antes de disparar o marcador; consumido no Awake (só no servidor).</summary>
        internal static SpawnCard PendingPayload;

        /// <summary>Efeito de impacto do Orbital Probe (CaptainAirstrikeImpact1): poeira, anéis, tremor.</summary>
        internal static GameObject LandingEffect;

        /// <summary>Escala do efeito: o original usa o raio da explosão do projétil (8).</summary>
        internal static float LandingEffectScale = 8f;

        /// <summary>Sons do pouso (tocados pelo servidor para todos os clientes).</summary>
        internal static NetworkSoundEventDef ImpactSound;
        internal static NetworkSoundEventDef LandingSound;

        private SpawnCard _payload;
        private float _dropTime;
        private float _age;
        private bool _done;

        private void Awake()
        {
            if (!NetworkServer.active)
                return;

            _payload = PendingPayload;
            PendingPayload = null;

            // O projétil se destrói ao fim do lifetime; o pouso acontece no último tick antes disso,
            // coincidindo com o projétil visual do ghost atingindo o chão.
            var explosion = GetComponent<ProjectileImpactExplosion>();
            _dropTime = explosion ? Mathf.Max(0.1f, explosion.lifetime - Time.fixedDeltaTime) : 1.9f;
        }

        /// <summary>Roda em todos os clientes (o projétil é sincronizado pela rede).</summary>
        private void Start() => Util.PlaySound(JetSounds.SupplyBeaconWarning, gameObject);

        private void FixedUpdate()
        {
            if (!NetworkServer.active || _done)
                return;

            _age += Time.fixedDeltaTime;
            if (_age < _dropTime)
                return;

            _done = true;
            if (!_payload || !Run.instance)
                return;

            Vector3 point = transform.position;
            var rng = new Xoroshiro128Plus(RoR2Application.rng.nextUlong);
            var spawned = InteractableSpawning.TrySpawnAt(_payload, point, rng);
            if (!spawned)
                return;

            if (LandingEffect)
            {
                EffectManager.SpawnEffect(LandingEffect, new EffectData
                {
                    origin = point,
                    rotation = Quaternion.identity,
                    scale = LandingEffectScale,
                }, transmit: true);
            }

            NetworkSounds.PlayAt(ImpactSound, point);
            NetworkSounds.PlayAt(LandingSound, point);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = JetTokens.SupplyDropMessage });
        }
    }
}
