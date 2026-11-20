using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Framework.Projectiles
{
    /// <summary>
    /// Explosão de impacto com dano separado para o ALVO DIRETO e para a ÁREA:
    /// - Alvo direto (o inimigo em que o projétil bateu): <see cref="DirectHitCount"/> acertos de
    ///   <see cref="DirectDamageMultiplier"/> × dano do projétil.
    /// - Área: 1 acerto de <see cref="AreaDamageMultiplier"/> × dano do projétil nos demais inimigos no raio
    ///   (o alvo direto fica de fora, a menos que <see cref="AreaHitsDirectTarget"/>).
    /// Um único efeito visual/som. O alvo direto é capturado por um <see cref="DirectHitTracker"/> no mesmo prefab.
    ///
    /// <see cref="IsApplyingDirectHit"/> fica true enquanto o dano direto é aplicado: como a morte é processada de forma
    /// síncrona dentro de TakeDamage, quem escuta onCharacterDeathServer sabe se a morte veio do acerto direto.
    /// </summary>
    public class DirectHitImpactExplosion : ProjectileImpactExplosion
    {
        public int DirectHitCount = 1;
        public float DirectDamageMultiplier = 1f;
        public float AreaDamageMultiplier = 1f;
        public bool AreaHitsDirectTarget;

        private DirectHitTracker _tracker;

        /// <summary>
        /// Servidor: disparado ao detonar, com a posição e a normal do impacto (normal "para cima" = caiu no chão).
        /// Use para efeitos pós-impacto (ex.: zona de fogo). Inscreva-se no Awake de um componente do mesmo prefab.
        /// </summary>
        public event System.Action<Vector3, Vector3> DetonatedServer;

        public bool IsApplyingDirectHit { get; private set; }

        // "public" porque o RoR2.dll é publicizado na compilação (no jogo é protected; ampliar o acesso é válido).
        public override void Awake()
        {
            base.Awake();
            _tracker = GetComponent<DirectHitTracker>();
            OnModifyBlastAttack += ConfigureAreaBlast;
        }

        public override void DetonateServer()
        {
            base.DetonateServer();
            DetonatedServer?.Invoke(transform.position, impactNormal);
        }

        public override void PerformDamage()
        {
            if (NetworkServer.active)
                ApplyDirectHits();
            base.PerformDamage();
        }

        private void ApplyDirectHits()
        {
            var target = _tracker ? _tracker.DirectTarget : null;
            if (!target || !target.alive || !projectileDamage)
                return;

            IsApplyingDirectHit = true;
            try
            {
                for (int i = 0; i < DirectHitCount && target.alive; i++)
                {
                    var damageInfo = new DamageInfo
                    {
                        damage = projectileDamage.damage * DirectDamageMultiplier,
                        crit = projectileDamage.crit,
                        attacker = projectileController.owner,
                        inflictor = gameObject,
                        position = _tracker.ImpactPoint,
                        force = projectileDamage.force * transform.forward,
                        procChainMask = projectileController.procChainMask,
                        procCoefficient = projectileController.procCoefficient,
                        damageType = projectileDamage.damageType,
                        damageColorIndex = projectileDamage.damageColorIndex,
                    };
                    target.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, target.gameObject);
                }
            }
            finally
            {
                IsApplyingDirectHit = false;
            }
        }

        private void ConfigureAreaBlast(BlastAttack blast)
        {
            blast.baseDamage = projectileDamage ? projectileDamage.damage * AreaDamageMultiplier : 0f;
            var directTarget = _tracker ? _tracker.DirectTarget : null;
            var owner = projectileController ? projectileController.owner : null;

            blast.attackerFiltering = AttackerFiltering.CustomFilter;
            blast.filterCallback = filter =>
                filter.healthComponent
                && (!owner || filter.healthComponent.gameObject != owner)
                && (AreaHitsDirectTarget || filter.healthComponent != directTarget);
        }
    }
}
