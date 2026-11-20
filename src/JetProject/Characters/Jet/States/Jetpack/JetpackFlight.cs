using EntityStates;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.Movement;
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

        /// <summary>
        /// A jetpack está voando agora (máquina "Jetpack" em voo e sem pausa)? Usado pelos visuais da jetpack.
        /// Nos clientes remotos a pausa não é calculada (só na autoridade), então lá conta como voando.
        /// </summary>
        public static bool IsActive(EntityStateMachine jetpackMachine) =>
            jetpackMachine && jetpackMachine.state is JetpackFlight flight && !flight.IsPaused;

        public override void OnEnter()
        {
            base.OnEnter();
            _fuel = GetComponent<JetpackFuel>();
            _bodyMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);
            _weaponMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Weapon);
            SetFlightGranted(true);

            // Sons em todos os clientes (o estado é sincronizado). O loop para no OnExit.
            Util.PlaySound(JetSounds.JetpackIgnite, gameObject);
            Util.PlaySound(JetSounds.JetpackLoop, gameObject);
            if (isAuthority && _fuel)
                _fuel.LowFuelReached += OnLowFuel;

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
            Util.PlaySound(JetSounds.JetpackLoopStop, gameObject);
            if (_fuel)
                _fuel.LowFuelReached -= OnLowFuel;
            base.OnExit();
        }

        /// <summary>Aviso local (só o dono ouve): o combustível é simulado apenas na autoridade.</summary>
        private void OnLowFuel() => Util.PlaySound(JetSounds.JetpackLowFuel, gameObject);

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
            Vector3 target = horizontal * FlightMotor.CruiseSpeed(characterBody) + Vector3.up * JetConfig.ClimbSpeed.Value;
            return target * BoostFactor();
        }

        private void SyncMotorDirection() => MotorMoveDirection = FlightMotor.MoveDirectionHolding(characterMotor);

        private float BoostFactor()
        {
            float duration = JetConfig.BoostDuration.Value;
            if (!WithBoost || duration <= 0f || fixedAge >= duration)
                return 1f;
            return Mathf.Lerp(JetConfig.BoostSpeedMultiplier.Value, 1f, fixedAge / duration);
        }

        private void SetFlightGranted(bool granted) => FlightMotor.SetGranted(characterMotor, ref _granted, granted);
    }
}
