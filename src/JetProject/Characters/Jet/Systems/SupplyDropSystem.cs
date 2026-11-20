using System.Collections.Generic;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.Projectiles;
using JetProject.Framework.Combat;
using JetProject.Framework.Interactables;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Characters.Jet.Systems
{
    /// <summary>
    /// "UES Safe Travels Supplies" no servidor. Cada disparo (<see cref="SupplyShot"/>) tem UM sorteio de baú,
    /// usado pela PRIMEIRA morte atribuída a ele, venha de onde vier:
    ///   - acerto direto ou área do sinalizador;
    ///   - a zona de fogo que ele deixa no chão;
    ///   - Burn aplicado por qualquer um dos dois (inclusive stacks novos depois do primeiro).
    /// Ao contar a morte: dispara o marcador de entrega (feixe do Captain) e sorteia o baú (ver <see cref="SupplyBeacon"/>).
    ///
    /// O dano de Burn não diz de onde o Burn veio, então inimigos atingidos pelo disparo são MARCADOS com ele;
    /// uma morte por Burn de um inimigo marcado conta para o disparo da última marcação.
    /// </summary>
    internal static class SupplyDropSystem
    {
        private const float MaxGroundDistance = 60f;

        private static readonly Dictionary<HealthComponent, SupplyShot> BurnedBy = new Dictionary<HealthComponent, SupplyShot>();
        private static bool _initialized;

        public static void Initialize()
        {
            if (_initialized)
                return;
            GlobalEventManager.onServerDamageDealt += OnServerDamageDealt;
            GlobalEventManager.onCharacterDeathServer += OnCharacterDeathServer;
            Stage.onServerStageBegin += _ => BurnedBy.Clear();
            _initialized = true;
        }

        /// <summary>Marca o alvo com o disparo e, na zona de fogo, sorteia o Burn (chance = proc coefficient).</summary>
        private static void OnServerDamageDealt(DamageReport report)
        {
            var marker = GetMarker(report?.damageInfo?.inflictor);
            if (!marker || marker.Shot == null || !report.victim)
                return;

            BurnedBy[report.victim] = marker.Shot;

            if (marker.IsFireZone && report.victim.alive && report.damageInfo.procCoefficient > 0f
                && Util.CheckRoll(report.damageInfo.procCoefficient * 100f, report.attackerMaster))
            {
                BurnHelper.Apply(report.attacker, report.victim.gameObject, report.damageInfo.damage,
                    report.damageInfo.inflictedHurtbox, report.damageInfo.procChainMask);
            }
        }

        private static void OnCharacterDeathServer(DamageReport report)
        {
            if (report?.victim == null)
                return;

            var shot = FindShot(report);
            BurnedBy.Remove(report.victim);
            if (shot == null || shot.ChestRolled || !ProjectileManager.instance)
                return;
            shot.ChestRolled = true;

            // Marcador e baú no chão EXATAMENTE abaixo de onde o inimigo morreu (inimigos voadores inclusive).
            Vector3 deathPoint = report.victimBody ? report.victimBody.footPosition : report.damageInfo.position;
            GroundFinder.TryFindBelow(deathPoint, MaxGroundDistance, out var position, out _);

            SupplyBeacon.PendingPayload = RollChest();
            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                projectilePrefab = SupplyBeaconProjectile.Prefab,
                position = position,
                rotation = Quaternion.identity,
                owner = report.attacker,
                damage = 0f,
                force = 0f,
                crit = false,
            });
            SupplyBeacon.PendingPayload = null;
        }

        /// <summary>Morte direta por projétil da secundária, ou por Burn de um inimigo marcado.</summary>
        private static SupplyShot FindShot(DamageReport report)
        {
            var marker = GetMarker(report.damageInfo?.inflictor);
            if (marker && marker.Shot != null)
                return marker.Shot;

            if (report.damageInfo != null && BurnHelper.IsBurn(report.damageInfo.dotIndex)
                && BurnedBy.TryGetValue(report.victim, out var shot))
                return shot;

            return null;
        }

        private static SupplyFlareMarker GetMarker(GameObject inflictor) =>
            inflictor ? inflictor.GetComponent<SupplyFlareMarker>() : null;

        private static SpawnCard RollChest()
        {
            float roll = Random.Range(0f, 100f);
            float legendary = JetConfig.SecondaryLegendaryChestChancePercent.Value;
            if (roll < legendary)
                return JetAssets.SupplyLegendaryChest;

            if (roll < legendary + JetConfig.SecondaryChestChancePercent.Value)
            {
                var chests = JetAssets.SupplyChests;
                for (int attempt = 0; attempt < chests.Count; attempt++)
                {
                    var candidate = chests[Random.Range(0, chests.Count)];
                    if (InteractableSpawning.IsAvailable(candidate))
                        return candidate;
                }
            }

            return null;
        }
    }
}
