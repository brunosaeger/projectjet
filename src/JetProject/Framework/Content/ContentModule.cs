using JetProject.Framework.Core;
using RoR2.ContentManagement;

namespace JetProject.Framework.Content
{
    /// <summary>Expõe o <see cref="ContentRegistry"/> como serviço e conecta o provider ao ContentManager do jogo.</summary>
    internal sealed class ContentModule : IModule
    {
        public string Name => "Content";

        public void Initialize(ModContext context)
        {
            var registry = new ContentRegistry();
            var provider = new JetContentPackProvider(registry);

            context.Register(registry);
            ContentManager.collectContentPackProviders += add => add(provider);
        }
    }
}
