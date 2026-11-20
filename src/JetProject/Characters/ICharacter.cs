using JetProject.Framework.Core;

namespace JetProject.Characters
{
    /// <summary>Contrato de um survivor do mod. Cada um monta e registra seu próprio conteúdo.</summary>
    internal interface ICharacter
    {
        string Name { get; }

        void Register(ModContext context);
    }
}
