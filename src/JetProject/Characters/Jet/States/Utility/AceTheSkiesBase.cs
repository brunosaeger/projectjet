using EntityStates;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.Movement;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Utility
{
    /// <summary>
    /// Base do Ace the Skies (máquina Body): voo em ALTA VELOCIDADE só para frente, seguindo a mira, com A/D
    /// desviando. Enquanto dura, o Jet não atira nem usa outras skills (a máquina Body fora do estado principal
    /// não processa inputs de skill) e os propulsores aparecem (<see cref="IJetpackThrustVisual"/>).
    /// Provisório: animação = Tactical Slide do Commando (a do Mercenário não existe no animador do Commando).
    /// </summary>
    public abstract class AceTheSkiesBase : BaseSkillState, IJetpackThrustVisual
    {
        private static readonly int SlideStateHash = Animator.StringToHash("SlideForward");
        private static readonly int SlideRateHash = Animator.StringToHash("SlideForward.playbackRate");
        private const float AnimationLoop = 0.8f;

        private bool _granted;
        private float _animationTimer;

        protected JetpackFuel Fuel { get; private set; }

        public abstract bool BlueFlame { get; }

        public override void OnEnter()
        {
            base.OnEnter();
            Fuel = GetComponent<JetpackFuel>();
            FlightMotor.SetGranted(characterMotor, ref _granted, true);
            if (isAuthority && characterMotor)
                characterMotor.Motor.ForceUnground();
            PlayLoopAnimation();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _animationTimer += Time.fixedDeltaTime;
            if (_animationTimer >= AnimationLoop)
                PlayLoopAnimation();
        }

        public override void OnExit()
        {
            FlightMotor.SetGranted(characterMotor, ref _granted, false);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.PrioritySkill;

        /// <summary>Autoridade: aplica a velocidade do Ace. <paramref name="allowClimb"/> falso = não sobe (só reto ou para baixo).</summary>
        protected void Fly(bool allowClimb)
        {
            if (!characterMotor || !inputBank)
                return;

            Vector3 aim = inputBank.aimDirection.normalized;
            Vector3 aimFlat = new Vector3(aim.x, 0f, aim.z);
            aimFlat = aimFlat.sqrMagnitude > 0.0001f ? aimFlat.normalized : transform.forward;
            Vector3 right = Vector3.Cross(Vector3.up, aimFlat);
            float lateral = Vector3.Dot(inputBank.moveVector, right);

            Vector3 direction = aim + right * (lateral * JetConfig.AceSteerStrength.Value);
            if (!allowClimb && direction.y > 0f)
                direction.y = 0f;
            direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : aimFlat;

            float speed = FlightMotor.CruiseSpeed(characterBody) * JetConfig.AceSpeedMultiplier.Value;
            characterMotor.velocity = direction * speed;
            characterMotor.moveDirection = FlightMotor.MoveDirectionHolding(characterMotor);
            if (characterDirection)
                characterDirection.moveVector = aimFlat;
        }

        private void PlayLoopAnimation()
        {
            _animationTimer = 0f;
            PlayAnimation("Body", SlideStateHash, SlideRateHash, AnimationLoop);
        }
    }
}
