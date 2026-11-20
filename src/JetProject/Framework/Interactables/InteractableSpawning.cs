using RoR2;
using RoR2.ExpansionManagement;
using UnityEngine;
using UnityEngine.Networking;

namespace JetProject.Framework.Interactables
{
    /// <summary>Spawn de interagíveis (baús, barris etc.) pelo DirectorCore. Só no servidor.</summary>
    internal static class InteractableSpawning
    {
        /// <summary>
        /// Coloca o interagível no ponto válido do chão mais próximo de <paramref name="position"/> (até <paramref name="maxDistance"/> m).
        /// Retorna null se não for servidor, se a expansão exigida estiver desligada ou se não houver espaço.
        /// </summary>
        public static GameObject TrySpawnNear(SpawnCard card, Vector3 position, Xoroshiro128Plus rng, float maxDistance = 10f)
        {
            if (!NetworkServer.active || !card || !DirectorCore.instance || !IsAvailable(card))
                return null;

            var rule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                position = position,
                minDistance = 0f,
                maxDistance = maxDistance,
            };
            return DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(card, rule, rng));
        }

        /// <summary>
        /// Coloca o interagível EXATAMENTE em <paramref name="position"/> (modo Direct, sem procurar nós próximos).
        /// A posição deve estar no chão (ver Combat/GroundFinder). Só no servidor.
        /// </summary>
        public static GameObject TrySpawnAt(SpawnCard card, Vector3 position, Xoroshiro128Plus rng)
        {
            if (!NetworkServer.active || !card || !DirectorCore.instance || !IsAvailable(card))
                return null;

            var rule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = position,
            };
            return DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(card, rule, rng));
        }

        /// <summary>Falso se o prefab exige uma expansão (DLC) que não está ativa na partida atual.</summary>
        public static bool IsAvailable(SpawnCard card)
        {
            if (!card || !card.prefab)
                return false;
            var requirement = card.prefab.GetComponent<ExpansionRequirementComponent>();
            return !requirement || !requirement.requiredExpansion || !Run.instance
                || Run.instance.IsExpansionEnabled(requirement.requiredExpansion);
        }
    }
}
