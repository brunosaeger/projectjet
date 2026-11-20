using UnityEngine;

namespace JetProject.Framework.Combat
{
    /// <summary>Limiares de coeficiente de dano que ativam itens do jogo.</summary>
    internal static class ProcThresholds
    {
        /// <summary>
        /// Kjaro's/Runald's Band ativam em acertos com coeficiente ≥ 4 (400%).
        /// Skills que dividem dano alto em vários acertos usam este teto para nunca ativá-las.
        /// </summary>
        public const float MaxCoefficientWithoutBandProc = 3.99f;

        public static float ClampBelowBandProc(float coefficient) => Mathf.Min(coefficient, MaxCoefficientWithoutBandProc);
    }
}
