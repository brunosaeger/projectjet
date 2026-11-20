using RoR2;
using UnityEngine;

namespace JetProject.Framework.Combat
{
    /// <summary>Explosões (BlastAttack) padronizadas, com efeito visual.</summary>
    internal static class BlastHelper
    {
        /// <summary>
        /// Explosão em área a partir de um body (atacante). Pode ser chamada na autoridade do cliente: o BlastAttack
        /// e o efeito (transmit) cuidam de levar ao servidor. Sem queda de dano com a distância.
        /// </summary>
        public static BlastAttack.Result Explode(CharacterBody attacker, Vector3 position, float radius, float damageCoefficient,
            DamageTypeCombo damageType, GameObject effect, float procCoefficient = 1f, float force = 1000f)
        {
            if (effect)
                EffectManager.SpawnEffect(effect, new EffectData { origin = position, scale = radius }, transmit: true);

            var blast = new BlastAttack
            {
                attacker = attacker.gameObject,
                inflictor = attacker.gameObject,
                teamIndex = attacker.teamComponent ? attacker.teamComponent.teamIndex : TeamIndex.None,
                position = position,
                radius = radius,
                baseDamage = attacker.damage * damageCoefficient,
                baseForce = force,
                crit = attacker.RollCrit(),
                procCoefficient = procCoefficient,
                falloffModel = BlastAttack.FalloffModel.None,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                damageType = damageType,
                damageColorIndex = DamageColorIndex.Default,
            };
            return blast.Fire();
        }
    }
}
