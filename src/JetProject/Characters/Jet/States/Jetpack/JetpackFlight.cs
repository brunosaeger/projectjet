using EntityStates;
using JetProject.Characters.Jet.Components;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Jetpack
{
    /// <summary>
    /// Voo com jetpack (máquina "Jetpack").
    /// Direção horizontal = WASD (relativo à câmera); a jetpack ligada sempre empurra um pouco para cima.
    /// A mira só gira o personagem (feito no <see cref="JetCharacterMain"/>), permitindo voar para trás atirando para frente.
    /// A velocidade acelera até o alvo (inércia), então religar a jetpack não dá velocidade instantânea.
    /// Termina ao soltar o pulo ou zerar o combustível.
    /// </summary>
    public class JetpackFlight : BaseState
    {
        /// <summary>Impulso inicial: só na primeira ignição depois de sair do chão (decidido por quem cria o estado).</summary>
        public bool WithBoost;

        private JetpackFuel _fuel;
        private EntityStateMachine _bodyMachine;
        private EntityStateMachine _weaponMachine;
        private bool _granted;

        /// <summary>Direção de movimento já escalada para o motor (lida pelo <see cref="JetCharacterMain"/>).</summary>
        public Vector3 MotorMoveDirection { get; private set; }

        /// <summary>Voo em pausa por outro estado (ver <see cref="IPausesJetpack"/>): não controla o movimento.</summary>
        public bool IsPaused { get; private set; }

        public override void OnEnter()
        {
            base.OnEnter();
            _fuel = GetComponent<JetpackFuel>();
            _bodyMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);
            _weaponMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Weapon);
            SetFlightGranted(true);

            if (isAuthority && characterMotor)
            {
                characterMotor.Motor.ForceUnground();
                if (WithBoost)
                    characterMotor.velocity = TargetVelocity();
                SyncMotorDirection();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority)
                return;

            if (!inputBank || !inputBank.jump.down || !_fuel || !_fuel.HasFuel)
            {
                outer.SetNextStateToMain();
                return;
            }

            // Pausa: outro estado manda no movimento (dash da Body, mira da secundária...). O voo não gasta combustível,
            // não mexe na velocidade e devolve a gravidade (necessário para a queda lenta da mira, p.ex.).
            IsPaused = ShouldPause();
            SetFlightGranted(!IsPaused);
            if (IsPaused)
            {
                MotorMoveDirection = Vector3.zero;
                return;
            }

            _fuel.Drain(Time.fixedDeltaTime);
            UpdateVelocity(Time.fixedDeltaTime);
        }

        public override void OnExit()
        {
            SetFlightGranted(false);
            MotorMoveDirection = Vector3.zero;
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Any;

        /// <summary>
        /// Pausa se a máquina Body não está no estado principal (ex.: dash) ou se qualquer máquina relevante
        /// está num estado marcado com <see cref="IPausesJetpack"/>.
        /// </summary>
        private bool ShouldPause()
        {
            var bodyState = _bodyMachine ? _bodyMachine.state : null;
            var weaponState = _weaponMachine ? _weaponMachine.state : null;
            return (bodyState != null && !(bodyState is JetCharacterMain))
                || bodyState is IPausesJetpack
                || weaponState is IPausesJetpack;
        }

        private void UpdateVelocity(float deltaTime)
        {
            if (!characterMotor || !inputBank)
                return;

            characterMotor.velocity = Vector3.MoveTowards(
                characterMotor.velocity, TargetVelocity(), JetConfig.FlightAcceleration.Value * deltaTime);

            if (characterMotor.isGrounded)
                characterMotor.Motor.ForceUnground();

            SyncMotorDirection();
        }

        private Vector3 TargetVelocity()
        {
            Vector3 horizontal = inputBank.moveVector;
            horizontal.y = 0f;
            if (horizontal.sqrMagnitude > 1f)
                horizontal.Normalize();

            // Regra de design: itens de velocidade escalam SÓ o horizontal (CruiseSpeed).
            // A subida é fixa (ClimbSpeed) e NÃO deve usar moveSpeed.
            Vector3 target = horizontal * CruiseSpeed() + Vector3.up * JetConfig.ClimbSpeed.Value;
            return target * BoostFactor();
        }

        /// <summary>
        /// O motor puxa a velocidade em direção a moveDirection × walkSpeed (CharacterMotor.PreMove).
        /// Alinhamos esse alvo com a velocidade atual para que só a nossa aceleração atue.
        /// </summary>
        private void SyncMotorDirection()
        {
            float walkSpeed = Mathf.Max(characterMotor.walkSpeed, 0.01f);
            MotorMoveDirection = characterMotor.velocity / walkSpeed;
        }

        /// <summary>Velocidade de movimento (com itens) sem o bônus de sprint, × multiplicador de voo.</summary>
        private float CruiseSpeed()
        {
            float moveSpeed = characterBody.moveSpeed;
            if (characterBody.isSprinting && characterBody.sprintingSpeedMultiplier > 0f)
                moveSpeed /= characterBody.sprintingSpeedMultiplier;
            return moveSpeed * JetConfig.FlightSpeedMultiplier.Value;
        }

        private float BoostFactor()
        {
            float duration = JetConfig.BoostDuration.Value;
            if (!WithBoost || duration <= 0f || fixedAge >= duration)
                return 1f;
            return Mathf.Lerp(JetConfig.BoostSpeedMultiplier.Value, 1f, fixedAge / duration);
        }

        /// <summary>Idempotente: só muda os contadores do motor quando o estado concedido muda.</summary>
        private void SetFlightGranted(bool granted)
        {
            if (!characterMotor || _granted == granted)
                return;
            _granted = granted;

            int delta = granted ? 1 : -1;

            var gravity = characterMotor.gravityParameters;
            gravity.channeledAntiGravityGranterCount += delta;
            characterMotor.gravityParameters = gravity;

            var flight = characterMotor.flightParameters;
            flight.channeledFlightGranterCount += delta;
            characterMotor.flightParameters = flight;
        }
    }
}
