using RoR2;
using UnityEngine;

namespace JetProject.Framework.Combat
{
    /// <summary>Queimadura (Burn) aplicada por código, idêntica ao IgniteOnHit do jogo.</summary>
    internal static class BurnHelper
    {
        /// <summary>
        /// Aplica Burn como o IgniteOnHit faz (GlobalEventManager.OnHitEnemy): dano total = 50% do dano do acerto,
        /// com upgrade para StrongerBurn se o atacante tiver Ignition Tank. Só no servidor.
        /// Use quando o Burn deve ter CHANCE (ex.: proc coefficient), já que o IgniteOnHit sempre aplica.
        /// </summary>
        public static void Apply(GameObject attacker, GameObject victim, float hitDamage, HurtBox hurtBox, ProcChainMask procChainMask)
        {
            if (!attacker || !victim)
                return;

            var dotInfo = new InflictDotInfo
            {
                attackerObject = attacker,
                victimObject = victim,
                totalDamage = hitDamage * 0.5f,
                damageMultiplier = 1f,
                dotIndex = DotController.DotIndex.Burn,
                hitHurtBox = hurtBox,
                procChainMask = procChainMask,
            };

            var attackerBody = attacker.GetComponent<CharacterBody>();
            if (attackerBody && attackerBody.inventory)
                StrengthenBurnUtils.CheckDotForUpgrade(attackerBody.inventory, ref dotInfo);

            DotController.InflictDot(ref dotInfo);
        }

        /// <summary>True para dano de queimadura (inclusive a versão do Ignition Tank).</summary>
        public static bool IsBurn(DotController.DotIndex dotIndex) =>
            dotIndex == DotController.DotIndex.Burn || dotIndex == DotController.DotIndex.StrongerBurn;
    }
}
