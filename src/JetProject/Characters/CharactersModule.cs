using JetProject.Framework.Content;
using JetProject.Framework.Core;

namespace JetProject.Characters
{
    /// <summary>
    /// Agrega os personagens do mod. Cada survivor vive em sua própria pasta (Characters/&lt;Nome&gt;/)
    /// e é registrado aqui. Por enquanto vazio: o survivor entra quando o conceito estiver definido (design/).
    /// </summary>
    internal sealed class CharactersModule : IModule
    {
        public string Name => "Characters";

        public void Initialize(ModContext context)
        {
            var content = context.Resolve<ContentRegistry>();
            _ = content; // usado quando o primeiro survivor for registrado
        }
    }
}
