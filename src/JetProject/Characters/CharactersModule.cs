using JetProject.Characters.Jet;
using JetProject.Framework.Core;

namespace JetProject.Characters
{
    /// <summary>Agrega os survivors do mod. Para adicionar um novo, inclua-o em <see cref="All"/>.</summary>
    internal sealed class CharactersModule : IModule
    {
        private static readonly ICharacter[] All =
        {
            new JetCharacter(),
        };

        public string Name => "Characters";

        public void Initialize(ModContext context)
        {
            foreach (var character in All)
            {
                character.Register(context);
                Log.Info($"[Characters] {character.Name} registrado.");
            }
        }
    }
}
