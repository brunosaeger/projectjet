using JetProject.Characters.Jet.Components;
using JetProject.Framework.Content;
using JetProject.Framework.Projectiles;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Characters.Jet.Projectiles
{
    /// <summary>
    /// Projétil da secundária: sinalizador em arco que explode em área e aplica Burn.
    /// Base PROVISÓRIA: firebolt da Artificer (visual do "ghost" vem dele).
    /// </summary>
    internal static class SupplyFlareProjectile
    {
        private const float Lifetime = 10f;

        public static GameObject Prefab { get; private set; }

        public static void Create(ContentRegistry content)
        {
            var prefab = ProjectileFactory.Clone(JetAssets.SupplyFlareBaseProjectileKey, "JetSupplyFlareProjectile");

            ProjectileFactory.MakeArcing(prefab, JetConfig.SecondaryProjectileSpeed.Value, Lifetime);
            ProjectileFactory.Scale(prefab, JetConfig.SecondaryProjectileScale.Value, JetConfig.SecondaryProjectileScale.Value);
            // Projétil (dano base = SecondaryDamageCoefficient): acerto direto N× 100%; área 1× AreaMultiplier, sem o alvo direto.
            ProjectileFactory.MakeExploding(prefab, JetConfig.SecondaryBlastRadius.Value, JetAssets.SupplyFlareExplosionEffect, Lifetime,
                directHitCount: JetConfig.SecondaryDirectHitCount.Value,
                directDamageMultiplier: 1f,
                areaDamageMultiplier: JetConfig.SecondaryAreaDamageCoefficient.Value / JetConfig.SecondaryDamageCoefficient.Value,
                areaHitsDirectTarget: JetConfig.SecondaryAreaHitsDirectTarget.Value);

            var damage = prefab.GetComponent<ProjectileDamage>();
            if (damage)
                damage.damageType = new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Secondary);

            prefab.AddComponent<SupplyFlareMarker>();
            prefab.AddComponent<SupplyFlareFireZoneSpawner>();

            LogBaseSounds(prefab);

            content.AddProjectile(prefab);
            Prefab = prefab;
        }

        /// <summary>Diagnóstico: sons que o projétil-base (firebolt) traz e que tocam/cortam junto com ele.</summary>
        private static void LogBaseSounds(GameObject prefab)
        {
            var controller = prefab.GetComponent<ProjectileController>();
            if (!controller)
                return;
            string loop = controller.flightSoundLoop ? controller.flightSoundLoop.startSoundName : "(nenhum)";
            Framework.Core.Log.Info($"[Jet] Sons do projétil-base do sinalizador: startSound='{controller.startSound}', flightLoop='{loop}'");
        }
    }
}
