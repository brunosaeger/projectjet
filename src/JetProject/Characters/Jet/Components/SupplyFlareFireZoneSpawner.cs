using JetProject.Characters.Jet.Projectiles;
using JetProject.Framework.Combat;
using JetProject.Framework.Projectiles;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// No sinalizador: ao detonar (servidor), cria a mini zona de fogo (estilo Molotov) no chão do ponto de impacto.
    /// Se explodiu no ar (ex.: acertou um inimigo voando), procura o chão logo abaixo; sem chão por perto, não cria.
    /// A zona herda o mesmo <see cref="Systems.SupplyShot"/> do sinalizador.
    /// </summary>
    public sealed class SupplyFlareFireZoneSpawner : MonoBehaviour
    {
        private const float MaxGroundDistance = 25f;

        private ProjectileController _controller;
        private ProjectileDamage _damage;
        private SupplyFlareMarker _marker;

        private void Awake()
        {
            _controller = GetComponent<ProjectileController>();
            _damage = GetComponent<ProjectileDamage>();
            _marker = GetComponent<SupplyFlareMarker>();

            var explosion = GetComponent<DirectHitImpactExplosion>();
            if (explosion)
                explosion.DetonatedServer += OnDetonated;
        }

        private void OnDetonated(Vector3 position, Vector3 impactNormal)
        {
            if (!SupplyFireZoneProjectile.Prefab || !ProjectileManager.instance || !_damage)
                return;
            if (!GroundFinder.TryFindBelow(position, MaxGroundDistance, out var ground, out _))
                return;

            // Dano do projétil da zona = dano por segundo desejado (a zona aplica 1× por segundo, em ticks).
            float perSecond = _damage.damage / JetConfig.SecondaryDamageCoefficient.Value * JetConfig.SecondaryFireZoneDamagePerSecond.Value;

            SupplyFlareMarker.PendingShot = _marker ? _marker.Shot : null;
            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                projectilePrefab = SupplyFireZoneProjectile.Prefab,
                position = ground,
                rotation = Quaternion.identity,
                owner = _controller ? _controller.owner : null,
                damage = perSecond,
                force = 0f,
                crit = _damage.crit,
            });
            SupplyFlareMarker.PendingShot = null;
        }
    }
}
