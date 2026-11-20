using EntityStates;
using EntityStates.Commando;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.States.Movement
{
    /// <summary>
    /// Rolagem/dash do Jet sobre o Tactical Dive do Commando (DodgeState), numa direção ESCOLHIDA pelo código
    /// (não pelas teclas), com a animação correspondente (frente/trás/esquerda/direita).
    /// Usos: dash para trás da secundária no ar, rolagens da utilitária, quique no chão após o Ace the Skies.
    ///
    /// Os números do dash vêm da configuração do DodgeState vanilla (EntityStateConfiguration), que o jogo só
    /// aplica ao tipo exato; por isso são copiados de uma instância configurada pelo catálogo.
    /// Roda na máquina "Body" (substitui o movimento normal; o voo da jetpack pausa sozinho).
    /// </summary>
    public class JetDodge : DodgeState
    {
        /// <summary>Direção do dash (mundo). Definida por quem cria o estado, na autoridade. Vertical é ignorada.</summary>
        public Vector3 Direction;

        /// <summary>Multiplica a distância (velocidade) do dash vanilla. 1 = igual ao Commando.</summary>
        public float DistanceMultiplier = 1f;

        /// <summary>Velocidade vertical inicial (m/s): &gt; 0 faz o dash sair em arco (a gravidade completa o arco).</summary>
        public float UpwardSpeed;

        /// <summary>
        /// Segue <see cref="Direction"/> em 3D durante todo o dash (inclusive para cima/baixo), em vez do movimento
        /// horizontal do Tactical Dive. Ex.: secundária no ar, dash na direção oposta à mira.
        /// </summary>
        public bool FollowFullDirection;

        private Vector3 _fullDirection;

        public override void OnEnter()
        {
            CopyVanillaDodgeTuning();
            initialSpeedCoefficient *= DistanceMultiplier;
            finalSpeedCoefficient *= DistanceMultiplier;

            // O dash vanilla também "recarrega" a primária do Commando; preservamos a munição da primária do Jet.
            var primary = skillLocator ? skillLocator.primary : null;
            int primaryStock = primary ? primary.stock : 0;

            base.OnEnter();

            if (primary)
                primary.stock = primaryStock;

            if (isAuthority)
                forwardDirection = Flatten(Direction);

            // A base escolhe a animação pelas teclas; refazemos pela direção final (vale em todos os clientes,
            // já que forwardDirection é sincronizado pelo DodgeState).
            PlayAnimation("Body", AnimationFor(forwardDirection), DodgeParamHash, duration);

            if (characterMotor)
            {
                Vector3 velocity = forwardDirection * rollSpeed;
                if (isAuthority && FollowFullDirection && Direction.sqrMagnitude > 0.0001f)
                {
                    _fullDirection = Direction.normalized;
                    velocity = _fullDirection * rollSpeed;
                    if (_fullDirection.y > 0f)
                        characterMotor.Motor.ForceUnground();
                }
                else if (isAuthority && UpwardSpeed > 0f)
                {
                    velocity.y = UpwardSpeed;
                    characterMotor.Motor.ForceUnground();
                }
                characterMotor.velocity = velocity;
                previousPosition = transform.position - characterMotor.velocity;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            // A base recalcula só o horizontal (e mantém subida); no modo 3D a direção completa manda durante o dash.
            if (isAuthority && characterMotor && _fullDirection != Vector3.zero)
                characterMotor.velocity = _fullDirection * rollSpeed;
        }

        private int AnimationFor(Vector3 direction)
        {
            Vector3 facing = characterDirection ? characterDirection.forward : direction;
            facing.y = 0f;
            Vector3 right = Vector3.Cross(Vector3.up, facing);
            float forwardAmount = Vector3.Dot(direction, facing.normalized);
            float rightAmount = Vector3.Dot(direction, right.normalized);

            if (Mathf.Abs(forwardAmount) >= Mathf.Abs(rightAmount))
                return forwardAmount >= 0f ? DodgeForwardStateHash : DodgeBackwardStateHash;
            return rightAmount >= 0f ? DodgeRightStateHash : DodgeLeftStateHash;
        }

        private void CopyVanillaDodgeTuning()
        {
            if (EntityStateCatalog.InstantiateState(typeof(DodgeState)) is DodgeState vanilla)
            {
                duration = vanilla.duration;
                initialSpeedCoefficient = vanilla.initialSpeedCoefficient;
                finalSpeedCoefficient = vanilla.finalSpeedCoefficient;
            }
        }

        private Vector3 Flatten(Vector3 direction)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
                direction = characterDirection ? -characterDirection.forward : -transform.forward;
            return direction.normalized;
        }

        /// <summary>Direção oposta à mira, em 3D (mirando para baixo = para cima). Usar com <see cref="FollowFullDirection"/>.</summary>
        public static Vector3 AwayFromAim(InputBankTest inputBank, Transform fallback) =>
            inputBank ? -inputBank.aimDirection.normalized : -fallback.forward;

        /// <summary>Direção "para trás" em relação à mira (horizontal).</summary>
        public static Vector3 BackwardFromAim(InputBankTest inputBank, Transform fallback)
        {
            Vector3 aim = inputBank ? inputBank.aimDirection : fallback.forward;
            aim.y = 0f;
            return aim.sqrMagnitude < 0.0001f ? -fallback.forward : -aim.normalized;
        }
    }
}
