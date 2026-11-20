using UnityEngine;

namespace JetProject.Framework.UI
{
    /// <summary>
    /// Um recurso que pode ser exibido no medidor ao lado do crosshair.
    /// Implementado por um componente no body; o <see cref="ResourceMeterHud"/> no mesmo body o encontra sozinho.
    /// </summary>
    internal interface IResourceMeterSource
    {
        /// <summary>Preenchimento de 0 a 1.</summary>
        float Fraction { get; }

        Color MeterColor { get; }

        bool MeterVisible { get; }
    }
}
