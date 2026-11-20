using RoR2;
using UnityEngine;

namespace JetProject.Framework.Combat
{
    /// <summary>
    /// Recuo que empurra o atirador para trás (mesmo mecanismo do tiro sem mira da Railgunner).
    /// Chamar apenas na autoridade do body: o movimento de jogador é simulado no cliente dono.
    /// No chão, o motor anula componentes verticais fracos (ground stick), então mirar para baixo não faz o
    /// personagem "pular"; no ar, o recuo vale em todas as direções. Não usar alwaysApply aqui.
    /// </summary>
    internal static class SelfKnockback
    {
        /// <param name="force">Força bruta; o deslocamento final é dividido pela massa do body (ex.: 100).</param>
        public static void Apply(CharacterBody body, Vector3 aimDirection, float force)
        {
            if (!body || !body.characterMotor || force <= 0f)
                return;
            body.characterMotor.ApplyForce(-force * aimDirection.normalized);
        }
    }
}
