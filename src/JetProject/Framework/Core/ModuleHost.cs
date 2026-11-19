using System;
using System.Collections.Generic;

namespace JetProject.Framework.Core
{
    /// <summary>Inicializa os módulos em ordem e isola falhas para que um módulo quebrado seja fácil de identificar no log.</summary>
    internal sealed class ModuleHost
    {
        private readonly ModContext _context;
        private readonly List<IModule> _modules = new List<IModule>();

        public ModuleHost(ModContext context) => _context = context;

        public void Add(IModule module) => _modules.Add(module);

        public void InitializeAll()
        {
            foreach (var module in _modules)
            {
                try
                {
                    module.Initialize(_context);
                    Log.Info($"[Module] {module.Name} inicializado.");
                }
                catch (Exception e)
                {
                    Log.Error($"[Module] Falha ao inicializar {module.Name}: {e}");
                    throw;
                }
            }
        }
    }
}
