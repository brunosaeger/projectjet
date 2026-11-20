using EntityStates;
using EntityStates.Commando;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Movement
{
    /// <summary>
    /// Dash para trás da secundária quando usada NO AR: o Tactical Dive do Commando (mesma animação e distância),
    /// mas SEMPRE para trás em relação à mira, independente das teclas.
    ///
    /// Os números do dash (duração/velocidades) vêm da configuração do DodgeState vanilla (EntityStateConfiguration),
    /// que o jogo só aplica ao tipo exato; por isso são copiados de uma instância configurada pelo catálogo.
    /// Roda na máquina "Body" (substitui o movimento normal durante o dash).
    /// </summary>
    public class SupplyFlareBackDodge : DodgeState
    {
        public override void OnEnter()
        {
            CopyVanillaDodgeTuning();

            // O dash vanilla também "recarrega" a primária do Commando; preservamos a munição da primária do Jet.
            var primary = skillLocator ? skillLocator.primary : null;
            int primaryStock = primary ? primary.stock : 0;

            base.OnEnter();

            if (primary)
                primary.stock = primaryStock;

            if (isAuthority)
                forwardDirection = BackwardFromAim();

            // A base escolhe a animação pelas teclas; forçamos a de "para trás" em todos os clientes.
            PlayAnimation("Body", DodgeBackwardStateHash, DodgeParamHash, duration);

            if (characterMotor)
            {
                characterMotor.velocity = forwardDirection * rollSpeed;
                previousPosition = transform.position - characterMotor.velocity;
            }
        }

        private void CopyVanillaDodgeTuning()
        {
            if (EntityStateCatalog.InstantiateState(typeof(DodgeState)) is DodgeState vanilla)
            {
                duration = vanilla.duration;
                initialSpeedCoefficient = vanilla.initialSpeedCoefficient;
                finalSpeedCoefficient = vanilla.finalSpeedCoefficient;
            }
        }

        private Vector3 BackwardFromAim()
        {
            Vector3 aim = inputBank ? inputBank.aimDirection : transform.forward;
            aim.y = 0f;
            if (aim.sqrMagnitude < 0.0001f && characterDirection)
                aim = characterDirection.forward;
            return -aim.normalized;
        }
    }
}
