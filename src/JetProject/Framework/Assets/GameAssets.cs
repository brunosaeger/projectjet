using UnityEngine;
using UnityEngine.AddressableAssets;

namespace JetProject.Framework.Assets
{
    /// <summary>
    /// Acesso aos assets do jogo base via Addressables.
    /// O jogo carrega por GUID, não por caminho: use SEMPRE as constantes de
    /// <c>RoR2BepInExPack.GameAssetPathsBetter</c> (ex.: <c>GameAssetPathsBetter.RoR2_Base_Commando.CommandoBody_prefab</c>).
    /// </summary>
    internal static class GameAssets
    {
        public static T Load<T>(string key) where T : Object
        {
            var asset = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
            if (asset == null)
                Core.Log.Error($"[Assets] Addressable não encontrado: {key}");
            return asset;
        }
    }
}
