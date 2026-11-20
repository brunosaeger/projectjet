using JetProject.Framework.Assets;
using R2API;
using UnityEngine;

namespace JetProject.Framework.Bodies
{
    /// <summary>Criação de prefabs de body e display a partir de prefabs existentes do jogo.</summary>
    internal static class BodyFactory
    {
        /// <summary>Clona um body do jogo com registro de rede (NetworkIdentity com assetId próprio).</summary>
        public static GameObject CloneBody(string addressableKey, string newName)
        {
            var original = GameAssets.Load<GameObject>(addressableKey);
            return original.InstantiateClone(newName, registerNetwork: true);
        }

        /// <summary>Clona um prefab apenas visual (ex.: display da tela de seleção), sem rede.</summary>
        public static GameObject CloneDisplay(string addressableKey, string newName)
        {
            var original = GameAssets.Load<GameObject>(addressableKey);
            return original.InstantiateClone(newName, registerNetwork: false);
        }
    }
}
