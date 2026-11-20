using EntityStates;
using JetProject.Characters.Jet.States.Movement;
using JetProject.Framework.Combat;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Utility
{
    /// <summary>
    /// Ativação da utilitária "DO A BARREL ROLL! // ACE THE SKIES!" (máquina Body, só no ar).
    /// Na autoridade: lê a direção pelas teclas (relativa à mira), explode no ponto de saída (600% + Burn) e passa para:
    ///  - A / D / S (ou nenhuma tecla = trás): <see cref="JetDodge"/> lateral/para trás, um pouco mais longo que o do Commando;
    ///  - W: <see cref="AceTheSkies"/> (dash para frente; segurando, vira o voo em alta velocidade).
    /// </summary>
    public class BarrelRollEntry : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (!isAuthority)
                return;

            BlastHelper.Explode(characterBody, characterBody.corePosition, JetConfig.RollBlastRadius.Value,
                JetConfig.RollDamageCoefficient.Value, UtilityFireDamage, JetAssets.FireExplosionEffect);

            if (IsForward(out var direction))
                outer.SetNextState(new AceTheSkies { activatorSkillSlot = activatorSkillSlot });
            else
                outer.SetNextState(new JetDodge { Direction = direction, DistanceMultiplier = JetConfig.RollDistanceMultiplier.Value });
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.PrioritySkill;

        internal static DamageTypeCombo UtilityFireDamage =>
            new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Utility);

        /// <summary>W → frente. Senão, a direção das teclas no plano horizontal (sem tecla = para trás da mira).</summary>
        private bool IsForward(out Vector3 direction)
        {
            Vector3 forward = inputBank ? inputBank.aimDirection : transform.forward;
            forward.y = 0f;
            forward = forward.sqrMagnitude > 0.0001f ? forward.normalized : transform.forward;
            Vector3 right = Vector3.Cross(Vector3.up, forward);

            Vector3 move = inputBank ? inputBank.moveVector : Vector3.zero;
            move.y = 0f;
            if (move.sqrMagnitude < 0.01f)
            {
                direction = -forward;
                return false;
            }

            float forwardAmount = Vector3.Dot(move, forward);
            float rightAmount = Vector3.Dot(move, right);
            if (Mathf.Abs(forwardAmount) >= Mathf.Abs(rightAmount))
            {
                direction = forwardAmount >= 0f ? forward : -forward;
                return forwardAmount >= 0f;
            }

            direction = rightAmount >= 0f ? right : -right;
            return false;
        }
    }
}
