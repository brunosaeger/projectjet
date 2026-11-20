using JetProject.Characters.Jet.Components;
using JetProject.Framework.Audio;
using JetProject.Framework.Content;
using JetProject.Framework.Projectiles;
using RoR2.Projectile;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;

namespace JetProject.Characters.Jet.Projectiles
{
    /// <summary>
    /// Marcador de entrega SEM DANO: clone do projétil do Orbital Probe do Captain (o feixe de aviso vem do ghost dele).
    /// O impacto original é neutralizado; o "pouso" (efeito + sons + baú) é feito pelo <see cref="SupplyBeacon"/>,
    /// só quando o baú cai.
    ///
    /// Estrutura do original (inspecionada no asset bundle do jogo, ror2-base-captain_static_assets):
    /// - ProjectileImpactExplosion: lifetime 2 s, blastRadius 8, explosionEffect VAZIO, impactEffect = CaptainAirstrikeImpact1.
    /// - CaptainAirstrikeImpact1: poeira/anéis/debris + ShakeEmitter (0,25 s, raio 70); soundName VAZIO.
    /// - O som do impacto é o evento Wwise Play_captain_shift_impact.
    /// </summary>
    internal static class SupplyBeaconProjectile
    {
        public static GameObject Prefab { get; private set; }

        public static void Create(ContentRegistry content)
        {
            var prefab = ProjectileFactory.Clone(RoR2_Base_Captain.CaptainAirstrikeProjectile1_prefab, "JetSupplyBeaconProjectile");

            var explosion = prefab.GetComponent<ProjectileImpactExplosion>();
            if (explosion)
            {
                SupplyBeacon.LandingEffect = explosion.impactEffect;
                SupplyBeacon.LandingEffectScale = explosion.blastRadius;

                explosion.blastDamageCoefficient = 0f;
                explosion.blastRadius = 0f;
                explosion.blastProcCoefficient = 0f;
                explosion.explosionEffect = null;
                explosion.impactEffect = null;
                explosion.fireChildren = false;
            }

            prefab.AddComponent<SupplyBeacon>();
            SupplyBeacon.ImpactSound = NetworkSounds.Create(content, JetSounds.SupplyBeaconImpact);
            SupplyBeacon.LandingSound = NetworkSounds.Create(content, JetSounds.SupplyDropLanding);

            content.AddProjectile(prefab);
            Prefab = prefab;
        }
    }
}
