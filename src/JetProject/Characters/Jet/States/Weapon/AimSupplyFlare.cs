using EntityStates;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.Projectiles;
using JetProject.Characters.Jet.States.Movement;
using JetProject.Framework.Audio;
using JetProject.Framework.Combat;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Weapon
{
    /// <summary>
    /// Secundária NO AR: segurar a tecla para mirar o sinalizador.
    /// Enquanto mira: queda bem lenta, combustível sem recarga, voo da jetpack pausado, mira do Captain +
    /// círculo no ponto de impacto (raio da explosão). Ao soltar: dispara (o arco cai exatamente no ponto mirado)
    /// e faz o dash para trás.
    ///
    /// A base (AimThrowableBase do jogo) calcula a trajetória, o indicador e dispara no OnExit.
    /// Provisório: animação de mira = Suppressive Fire do Commando.
    /// </summary>
    public class AimSupplyFlare : AimThrowableBase, IPausesJetpack
    {
        private static readonly int AimAnimationHash = Animator.StringToHash("FireBarrage");
        private static readonly int AimAnimationRateHash = Animator.StringToHash("FireBarrage.playbackRate");
        private static readonly int FireAnimationHash = Animator.StringToHash("FireFMJ");
        private static readonly int FireAnimationRateHash = Animator.StringToHash("FireFMJ.playbackRate");
        private const float AimAnimationLoop = 1.5f;
        private const float FireAnimationDuration = 0.4f;

        private JetpackFuel _fuel;
        private CrosshairUtils.OverrideRequest _crosshairOverride;
        private float _aimAnimationTimer;

        public override void OnEnter()
        {
            maxDistance = JetConfig.SecondaryAimMaxDistance.Value;
            rayRadius = 0.5f;
            projectilePrefab = SupplyFlareProjectile.Prefab;
            endpointVisualizerPrefab = JetAssets.SupplyFlareAreaIndicator;
            endpointVisualizerRadiusScale = 0f;   // 0 = a base usa o raio da explosão do projétil
            baseMinimumDuration = 0.1f;
            damageCoefficient = ProcThresholds.ClampBelowBandProc(JetConfig.SecondaryDamageCoefficient.Value);

            base.OnEnter();

            // A base esconde o crosshair quando há indicador; aqui queremos a mira do Captain + o círculo.
            characterBody.hideCrosshair = false;
            if (JetAssets.SupplyFlareAimCrosshair)
                _crosshairOverride = CrosshairUtils.RequestOverrideForBody(characterBody, JetAssets.SupplyFlareAimCrosshair, CrosshairUtils.OverridePriority.Skill);

            _fuel = GetComponent<JetpackFuel>();
            if (_fuel)
                _fuel.BlockRegen();

            PlayAimAnimation();
        }

        public override void Update()
        {
            base.Update();
            if (characterBody)
                characterBody.hideCrosshair = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            StartAimMode(2f);

            _aimAnimationTimer += Time.fixedDeltaTime;
            if (_aimAnimationTimer >= AimAnimationLoop)
                PlayAimAnimation();

            // Queda lenta: limita a velocidade de descida (subida não é afetada).
            if (isAuthority && characterMotor && !characterMotor.isGrounded)
            {
                float maxFall = JetConfig.SecondaryAimFallSpeed.Value;
                if (characterMotor.velocity.y < -maxFall)
                    characterMotor.velocity.y = -maxFall;
            }
        }

        public override void OnExit()
        {
            _crosshairOverride?.Dispose();
            _crosshairOverride = null;
            if (_fuel)
                _fuel.UnblockRegen();
            base.OnExit();   // a base dispara o projétil aqui
        }

        /// <summary>Autoridade: dispara (via base) e em seguida faz o dash para trás.</summary>
        public override void FireProjectile()
        {
            base.FireProjectile();

            var body = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);
            if (body)
                body.SetInterruptState(new JetDodge { Direction = JetDodge.AwayFromAim(inputBank, transform), FollowFullDirection = true }, InterruptPriority.Skill);
        }

        /// <summary>Todos os clientes: feedback do disparo.</summary>
        public override void OnProjectileFiredLocal()
        {
            DetachedSound.Play(JetSounds.SupplyFlareFire, transform.position, layers: JetConfig.SecondaryFireSoundLayers.Value);
            PlayAnimation("Gesture, Additive", FireAnimationHash, FireAnimationRateHash, FireAnimationDuration);
            PlayAnimation("Gesture, Override", FireAnimationHash, FireAnimationRateHash, FireAnimationDuration);

            float recoil = JetConfig.SecondaryCameraRecoil.Value;
            AddRecoil(-0.6f * recoil, -1f * recoil, -0.3f * recoil, 0.3f * recoil);
        }

        private void PlayAimAnimation()
        {
            _aimAnimationTimer = 0f;
            PlayCrossfade("Gesture, Additive", AimAnimationHash, AimAnimationRateHash, AimAnimationLoop, 0.2f);
            PlayCrossfade("Gesture, Override", AimAnimationHash, AimAnimationRateHash, AimAnimationLoop, 0.2f);
        }
    }
}
