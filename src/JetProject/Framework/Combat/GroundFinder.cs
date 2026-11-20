using RoR2;
using UnityEngine;

namespace JetProject.Framework.Combat
{
    /// <summary>Encontrar o chão abaixo de um ponto (para zonas, poças de fogo etc.).</summary>
    internal static class GroundFinder
    {
        /// <summary>Raycast para baixo no cenário (world), a partir de um pouco acima de <paramref name="from"/>.</summary>
        public static bool TryFindBelow(Vector3 from, float maxDistance, out Vector3 point, out Vector3 normal)
        {
            const float lift = 0.5f;
            if (Physics.Raycast(from + Vector3.up * lift, Vector3.down, out var hit, maxDistance + lift, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                point = hit.point;
                normal = hit.normal;
                return true;
            }

            point = from;
            normal = Vector3.up;
            return false;
        }
    }
}
