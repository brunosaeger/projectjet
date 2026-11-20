using EntityStates;
using JetProject.Characters.Jet.Projectiles;
using JetProject.Characters.Jet.States.Movement;
using JetProject.Framework.Audio;
using JetProject.Framework.Combat;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Weapon
{
    /// <summary>
    /// Secundária "UES Safe Travels Supplies": dispara com a outra mão um sinalizador em arco que explode em área
    /// e aplica Burn. Recuo INTENSO no Jet (ferramenta para se manter no ar). Baús: ver SupplyDropSystem.
    /// Provisório: animação do FMJ do Commando e som de disparo da Artificer.
    /// </summary>
    public class FireSupplyFlare : BaseSkillState
    {
        private const float BaseDuration = 0.4f;
        private const string Muzzle = "MuzzleLeft";
        private static readonly int FireAnimationHash = Animator.StringToHash("FireFMJ");
        private static readonly int FireAnimationRateHash = Animator.StringToHash("FireFMJ.playbackRate");

        private float _duration;

        public override void OnEnter()
        {
            base.OnEnter();
            _duration = BaseDuration / attackSpeedStat;

            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f);
            PlayAnimation("Gesture, Additive", FireAnimationHash, FireAnimationRateHash, _duration);
            PlayAnimation("Gesture, Override", FireAnimationHash, FireAnimationRateHash, _duration);
            DetachedSound.Play(JetSounds.SupplyFlareFire, transform.position, layers: JetConfig.SecondaryFireSoundLayers.Value);

            float recoil = JetConfig.SecondaryCameraRecoil.Value;
            AddRecoil(-0.6f * recoil, -1f * recoil, -0.3f * recoil, 0.3f * recoil);

            if (isAuthority)
            {
                Fire(aimRay);
                ApplyRecoilMovement(aimRay);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= _duration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        /// <summary>No chão: recuo forte (empurrão). No ar: dash para trás do Commando (sempre para trás).</summary>
        private void ApplyRecoilMovement(Ray aimRay)
        {
            bool airborne = characterMotor && !characterMotor.isGrounded;
            var body = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);

            if (airborne && JetConfig.SecondaryAirBackDodge.Value && body)
                body.SetInterruptState(new JetDodge { Direction = JetDodge.AwayFromAim(inputBank, transform), FollowFullDirection = true }, InterruptPriority.Skill);
            else
                SelfKnockback.Apply(characterBody, aimRay.direction, JetConfig.SecondarySelfKnockback.Value);
        }

        private void Fire(Ray aimRay)
        {
            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
            {
                projectilePrefab = SupplyFlareProjectile.Prefab,
                position = aimRay.origin,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                owner = gameObject,
                damage = ProcThresholds.ClampBelowBandProc(JetConfig.SecondaryDamageCoefficient.Value) * damageStat,
                force = 1500f,
                crit = RollCrit(),
            });
        }
    }
}
