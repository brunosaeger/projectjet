using JetProject.Characters.Jet.States.Movement;
using JetProject.Framework.Combat;
using JetProject.Framework.Skills;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Utility
{
    /// <summary>
    /// Ace the Skies, fase de QUEIMA (chama AZUL): o combustível é gasto no ritmo do voo (× AceBurnDrainMultiplier)
    /// e cada gota queimada aumenta o dano final. Termina ao soltar o Shift ou ao tocar
    /// o chão; com o tanque vazio o Jet continua o voo, mas não consegue mais subir (só reto/para baixo).
    /// Ao terminar:
    ///  - explosão de 1200% a 3600%, proporcional à fração do tanque queimada nesta fase (Burn; ativa Kjaro/Runald);
    ///  - a skill perde TODAS as cargas e a primeira só volta após <c>AceBurnLockout</c> (10 s);
    ///  - a jetpack fica desativada pelo mesmo tempo;
    ///  - 1 stack de Burn fraco no próprio Jet (estético);
    ///  - se terminou batendo no chão: quica para trás em arco (rolagem para trás do Commando).
    /// </summary>
    public class AceTheSkiesBurn : AceTheSkiesBase
    {
        private const float MinTimeBeforeGroundCheck = 0.1f;

        private float _burnedSeconds;
        private bool _finished;

        public override bool BlueFlame => true;

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

            bool hasFuel = Fuel && Fuel.HasFuel;
            Fly(allowClimb: hasFuel);

            if (hasFuel)
            {
                float before = Fuel.Current;
                Fuel.Drain(JetConfig.AceBurnDrainMultiplier.Value * Time.fixedDeltaTime);
                _burnedSeconds += before - Fuel.Current;
            }

            if (characterMotor && characterMotor.isGrounded && fixedAge >= MinTimeBeforeGroundCheck)
                Finish(hitGround: true);
            else if (!IsKeyDownAuthority())
                Finish(hitGround: false);
        }

        private void Finish(bool hitGround)
        {
            if (_finished)
                return;
            _finished = true;

            float burnedFraction = Fuel && Fuel.Capacity > 0f ? Mathf.Clamp01(_burnedSeconds / Fuel.Capacity) : 0f;
            float damage = Mathf.Lerp(JetConfig.AceBurnMinDamage.Value, JetConfig.AceBurnMaxDamage.Value, burnedFraction);

            BlastHelper.Explode(characterBody, characterBody.corePosition, JetConfig.AceBurnBlastRadius.Value, damage,
                BarrelRollEntry.UtilityFireDamage, JetAssets.FireExplosionEffect, force: 3000f);

            float lockout = JetConfig.AceBurnLockout.Value;
            SkillLockout.EmptyAndDelay(activatorSkillSlot, lockout);
            if (Fuel)
                Fuel.DisableFor(lockout);
            BurnHelper.Apply(gameObject, gameObject, characterBody.damage * JetConfig.AceSelfBurnDamage.Value, null, default);

            if (hitGround)
            {
                outer.SetNextState(new JetDodge
                {
                    Direction = JetDodge.BackwardFromAim(inputBank, transform),
                    UpwardSpeed = JetConfig.AceBounceUpSpeed.Value,
                });
            }
            else
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
