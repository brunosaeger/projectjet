using System.Collections;
using RoR2;
using RoR2.ContentManagement;

namespace JetProject.Framework.Content
{
    /// <summary>Adaptador entre o <see cref="ContentRegistry"/> e o sistema de ContentPacks do RoR2.</summary>
    internal sealed class JetContentPackProvider : IContentPackProvider
    {
        private readonly ContentPack _contentPack = new ContentPack();
        private readonly ContentRegistry _registry;

        public JetContentPackProvider(ContentRegistry registry)
        {
            _registry = registry;
            _contentPack.identifier = identifier;
        }

        public string identifier => PluginInfo.Guid;

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            _registry.PopulateAndSeal(_contentPack);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(_contentPack, args.output);
            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
