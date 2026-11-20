using JetProject.Characters.Jet.Components;
using JetProject.Framework.Content;
using JetProject.Framework.Projectiles;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.Projectiles
{
    /// <summary>
    /// Mini poça de fogo deixada pelo sinalizador ao cair (clone menor da zona do Molotov).
    /// Dano por segundo com proc coefficient baixo; o Burn NÃO vem do IgniteOnHit (que sempre aplica): o
    /// SupplyDropSystem sorteia o Burn com chance = proc coefficient.
    /// </summary>
    internal static class SupplyFireZoneProjectile
    {
        public static GameObject Prefab { get; private set; }

        public static void Create(ContentRegistry content)
        {
            var prefab = ProjectileFactory.Clone(JetAssets.SupplyFireZoneBaseKey, "JetSupplyFireZone");

            ProjectileFactory.ConfigureDamageZone(prefab,
                damagePerSecondMultiplier: 1f,   // o dano/s já vem no dano do projétil (ver SupplyFlareFireZoneSpawner)
                procCoefficient: JetConfig.SecondaryFireZoneProcCoefficient.Value,
                lifetime: JetConfig.SecondaryFireZoneDuration.Value,
                scale: JetConfig.SecondaryFireZoneScale.Value,
                damageType: new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Secondary));

            prefab.AddComponent<SupplyFlareMarker>().IsFireZone = true;

            content.AddProjectile(prefab);
            Prefab = prefab;
        }
    }
}
