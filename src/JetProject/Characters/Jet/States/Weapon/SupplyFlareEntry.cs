using EntityStates;

namespace JetProject.Characters.Jet.States.Weapon
{
    /// <summary>
    /// Estado de ativação da secundária: só decide o caminho (na autoridade) e passa adiante.
    /// No chão → <see cref="FireSupplyFlare"/> (disparo imediato). No ar → <see cref="AimSupplyFlare"/> (mira).
    /// Não toca som/animação: assim os outros clientes não "veem" um disparo que não aconteceu.
    /// </summary>
    public class SupplyFlareEntry : BaseSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (!isAuthority)
                return;

            bool airborne = characterMotor && !characterMotor.isGrounded;
            if (airborne && JetConfig.SecondaryAirAim.Value)
                outer.SetNextState(new AimSupplyFlare { activatorSkillSlot = activatorSkillSlot });
            else
                outer.SetNextState(new FireSupplyFlare { activatorSkillSlot = activatorSkillSlot });
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}
