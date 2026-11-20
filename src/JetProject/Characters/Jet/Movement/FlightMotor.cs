using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.Movement
{
    /// <summary>
    /// Controle de voo direto no CharacterMotor, compartilhado pelos estados que pilotam o Jet no ar
    /// (voo da jetpack, Ace the Skies).
    /// </summary>
    internal static class FlightMotor
    {
        /// <summary>
        /// Liga/desliga voo + antigravidade no motor (contadores do jogo). Idempotente via <paramref name="current"/>:
        /// cada estado guarda o próprio flag e só altera os contadores quando ele muda.
        /// </summary>
        public static void SetGranted(CharacterMotor motor, ref bool current, bool granted)
        {
            if (!motor || current == granted)
                return;
            current = granted;
            int delta = granted ? 1 : -1;

            var gravity = motor.gravityParameters;
            gravity.channeledAntiGravityGranterCount += delta;
            motor.gravityParameters = gravity;

            var flight = motor.flightParameters;
            flight.channeledFlightGranterCount += delta;
            motor.flightParameters = flight;
        }

        /// <summary>
        /// O motor puxa a velocidade em direção a moveDirection × walkSpeed (CharacterMotor.PreMove).
        /// Devolve o moveDirection que mantém a velocidade atual, para só a nossa lógica mexer nela.
        /// </summary>
        public static Vector3 MoveDirectionHolding(CharacterMotor motor) =>
            motor.velocity / Mathf.Max(motor.walkSpeed, 0.01f);

        /// <summary>Velocidade de movimento (com itens) sem o bônus de sprint, × multiplicador de voo da jetpack.</summary>
        public static float CruiseSpeed(CharacterBody body)
        {
            float moveSpeed = body.moveSpeed;
            if (body.isSprinting && body.sprintingSpeedMultiplier > 0f)
                moveSpeed /= body.sprintingSpeedMultiplier;
            return moveSpeed * JetConfig.FlightSpeedMultiplier.Value;
        }
    }
}
