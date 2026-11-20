using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using JetProject.Framework.Combat;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Characters.Jet.States.Weapon
{
    /// <summary>
    /// Primária: pistola pesada semiautomática, com bala grossa e recuo em todo tiro.
    /// O SteppedSkillDef informa o passo do combo (0, 1, 2); o 3º tiro (passo 2) perfura, acerta 2× 250% e tem recuo maior.
    /// Provisório: som/animação da pistola do Commando e tracers de Bandit/Railgunner até o Jet ter assets próprios.
    /// </summary>
    public class FireHandCannon : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public const int StepCount = 3;
        private const int PiercingStep = StepCount - 1;
        private const string Muzzle = "MuzzleRight";
        private static readonly int FireAnimationHash = Animator.StringToHash("FirePistol, Right");

        private int _step;
        private float _duration;

        private bool IsPiercing => _step == PiercingStep;

        void SteppedSkillDef.IStepSetter.SetStep(int i) => _step = i;

        public override void OnEnter()
        {
            base.OnEnter();
            _duration = JetConfig.PrimaryBaseDuration.Value / attackSpeedStat;

            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f);
            PlayAnimation("Gesture Additive, Right", FireAnimationHash);
            PlayFireSound();
            if (FirePistol2.muzzleEffectPrefab)
                EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, gameObject, Muzzle, false);

            // Coice da arma: a câmera sobe (vertical negativo = para cima) com leve desvio lateral e volta sozinha.
            float recoil = IsPiercing ? JetConfig.PrimaryPiercingCameraRecoil.Value : JetConfig.PrimaryCameraRecoil.Value;
            AddRecoil(-0.6f * recoil, -1f * recoil, -0.3f * recoil, 0.3f * recoil);

            if (isAuthority)
            {
                Fire(aimRay);
                float knockback = IsPiercing ? JetConfig.PrimaryPiercingSelfKnockback.Value : JetConfig.PrimarySelfKnockback.Value;
                SelfKnockback.Apply(characterBody, aimRay.direction, knockback);
            }

            characterBody.AddSpreadBloom(0.6f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && fixedAge >= _duration)
                outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(_step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            _step = reader.ReadInt32();
        }

        /// <summary>Roda em todos os clientes (OnEnter), por isso não precisa de sincronização de rede.</summary>
        private void PlayFireSound() =>
            Util.PlaySound(IsPiercing ? JetSounds.HandCannonFirePiercing : JetSounds.HandCannonFire, gameObject);

        /// <summary>
        /// Tiro normal: 1 acerto. 3º tiro: N acertos na MESMA linha (sem spread), cada um abaixo de 400%,
        /// para somar dano alto sem ativar itens de "acerto ≥ 400%" (Kjaro's/Runald's Band).
        /// </summary>
        private void Fire(Ray aimRay)
        {
            int hits = IsPiercing ? Mathf.Max(1, JetConfig.PrimaryPiercingHitCount.Value) : 1;
            float coefficient = IsPiercing ? JetConfig.PrimaryPiercingDamageCoefficient.Value : JetConfig.PrimaryDamageCoefficient.Value;
            coefficient = ProcThresholds.ClampBelowBandProc(coefficient);

            Vector3 direction = IsPiercing ? aimRay.direction : Spread(aimRay.direction);
            for (int i = 0; i < hits; i++)
                CreateBullet(aimRay.origin, direction, coefficient, showTracer: i == 0).Fire();
        }

        private BulletAttack CreateBullet(Vector3 origin, Vector3 direction, float coefficient, bool showTracer)
        {
            var bullet = new BulletAttack
            {
                owner = gameObject,
                weapon = gameObject,
                origin = origin,
                aimVector = direction,
                minSpread = 0f,
                maxSpread = 0f,
                damage = coefficient * damageStat,
                procCoefficient = 1f,
                force = IsPiercing ? 600f : 300f,
                tracerEffectPrefab = !showTracer ? null : IsPiercing ? JetAssets.HandCannonPiercingTracer : JetAssets.HandCannonTracer,
                hitEffectPrefab = FirePistol2.hitEffectPrefab,
                muzzleName = Muzzle,
                isCrit = RollCrit(),
                radius = IsPiercing ? JetConfig.PrimaryPiercingBulletRadius.Value : JetConfig.PrimaryBulletRadius.Value,
                smartCollision = true,
                damageType = DamageTypeCombo.GenericPrimary,
            };

            if (IsPiercing)
                bullet.stopperMask = LayerIndex.world.mask;

            return bullet;
        }

        /// <summary>Desvio aleatório dentro do bloom atual (feito uma vez, fora da BulletAttack).</summary>
        private Vector3 Spread(Vector3 direction) =>
            Util.ApplySpread(direction, 0f, characterBody.spreadBloomAngle, 1f, 1f);
    }
}
