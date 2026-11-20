using EntityStates;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.States.Jetpack;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States
{
    /// <summary>
    /// Estado principal (máquina "Body") do Jet.
    /// No chão, o pulo é normal. No ar, apertar pulo liga a jetpack (sem double jump, nem com Hopoo Feather).
    /// O impulso inicial só vale na primeira ignição desde que saiu do chão; religar no ar não dá impulso.
    /// Durante o voo: repassa ao motor a direção do <see cref="JetpackFlight"/> e vira o corpo para a mira.
    /// </summary>
    public class JetCharacterMain : GenericCharacterMain
    {
        private EntityStateMachine _jetpackMachine;
        private JetpackFuel _fuel;
        private bool _boostAvailable = true;

        public override void OnEnter()
        {
            base.OnEnter();
            _jetpackMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Jetpack);
            _fuel = GetComponent<JetpackFuel>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && characterMotor && characterMotor.isGrounded && !IsFlying(out _))
                _boostAvailable = true;
        }

        public override void ProcessJump()
        {
            if (!jumpInputReceived || !characterMotor)
                return;

            if (characterMotor.isGrounded)
            {
                base.ProcessJump();
                return;
            }

            TryIgniteJetpack();
        }

        public override void HandleMovements()
        {
            base.HandleMovements();
            if (!isAuthority || !characterMotor || !IsFlying(out var flight) || flight.IsPaused)
                return;

            characterMotor.moveDirection = flight.MotorMoveDirection;
            if (characterDirection)
                characterDirection.moveVector = new Vector3(aimDirection.x, 0f, aimDirection.z);
        }

        private bool IsFlying(out JetpackFlight flight)
        {
            flight = _jetpackMachine ? _jetpackMachine.state as JetpackFlight : null;
            return flight != null;
        }

        private void TryIgniteJetpack()
        {
            if (!_jetpackMachine || !_fuel || !_fuel.CanIgnite || IsFlying(out _))
                return;

            _jetpackMachine.SetNextState(new JetpackFlight { WithBoost = _boostAvailable });
            _boostAvailable = false;
        }
    }
}
