using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Utility
{
    /// <summary>
    /// Ace the Skies, fase NORMAL: dash/voo rápido para frente gastando combustível no ritmo normal do voo.
    /// - Solto antes de <c>AceMinDuration</c>: ainda faz um dash curto (é a "rolagem" do W).
    /// - Solto depois: termina.
    /// - Segurado até <c>AceBurnStartTime</c>: entra na QUEIMA (<see cref="AceTheSkiesBurn"/>).
    /// - Combustível acabou nesta fase: termina (cai normalmente).
    /// </summary>
    public class AceTheSkies : AceTheSkiesBase
    {
        public override bool BlueFlame => false;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(JetSounds.JetpackIgnite, gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
                return;

            Fly(allowClimb: true);
            if (Fuel)
                Fuel.Drain(Time.fixedDeltaTime);

            bool released = !IsKeyDownAuthority();
            if ((released && fixedAge >= JetConfig.AceMinDuration.Value) || (Fuel && !Fuel.HasFuel))
            {
                outer.SetNextStateToMain();
                return;
            }

            if (!released && fixedAge >= JetConfig.AceBurnStartTime.Value)
                outer.SetNextState(new AceTheSkiesBurn { activatorSkillSlot = activatorSkillSlot });
        }
    }
}
