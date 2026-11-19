using JetProject.Framework.Core;

namespace JetProject.Framework.Assets
{
    internal sealed class AssetsModule : IModule
    {
        public string Name => "Assets";

        public void Initialize(ModContext context)
        {
            context.Register(new AssetBundleService(context.PluginDirectory));
        }
    }
}
