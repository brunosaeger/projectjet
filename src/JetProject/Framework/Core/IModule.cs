namespace JetProject.Framework.Core
{
    /// <summary>
    /// Unidade de inicialização do mod. Cada módulo cuida de uma responsabilidade e
    /// conversa com os outros apenas por serviços registrados no <see cref="ModContext"/>.
    /// </summary>
    internal interface IModule
    {
        string Name { get; }

        /// <summary>Chamado uma vez, no Awake do plugin, na ordem de registro do <see cref="ModuleHost"/>.</summary>
        void Initialize(ModContext context);
    }
}
