using System.Collections.Generic;
using System.IO;
using JetProject.Framework.Core;
using UnityEngine;

namespace JetProject.Framework.Assets
{
    /// <summary>
    /// Carrega (com cache) os AssetBundles gerados pelo projeto Unity em Resources/AssetBundles.
    /// Módulos de conteúdo pedem assets por nome de bundle e de asset, sem saber de caminhos.
    /// </summary>
    internal sealed class AssetBundleService
    {
        private readonly string _bundleDirectory;
        private readonly Dictionary<string, AssetBundle> _loaded = new Dictionary<string, AssetBundle>();

        public AssetBundleService(string pluginDirectory)
        {
            _bundleDirectory = Path.Combine(pluginDirectory, "Resources", "AssetBundles");
        }

        public AssetBundle GetBundle(string bundleName)
        {
            if (_loaded.TryGetValue(bundleName, out var bundle))
                return bundle;

            var path = Path.Combine(_bundleDirectory, bundleName);
            if (!File.Exists(path))
            {
                Log.Error($"[Assets] Bundle não encontrado: {path}");
                return null;
            }

            bundle = AssetBundle.LoadFromFile(path);
            _loaded[bundleName] = bundle;
            return bundle;
        }

        public T Load<T>(string bundleName, string assetName) where T : Object
        {
            var asset = GetBundle(bundleName)?.LoadAsset<T>(assetName);
            if (asset == null)
                Log.Error($"[Assets] Asset '{assetName}' ({typeof(T).Name}) não encontrado em '{bundleName}'.");
            return asset;
        }
    }
}
