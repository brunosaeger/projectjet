using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace JetProject.Framework.Core
{
    /// <summary>
    /// Contexto compartilhado entre módulos: dados do plugin e um registro de serviços simples.
    /// Um módulo expõe sua API com <see cref="Register{T}"/>, e os outros a obtêm com <see cref="Resolve{T}"/>.
    /// </summary>
    internal sealed class ModContext
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public ModContext(string pluginDirectory, ConfigFile config)
        {
            PluginDirectory = pluginDirectory;
            Config = config;
        }

        /// <summary>Pasta onde a DLL está instalada (BepInEx/plugins/JetProject).</summary>
        public string PluginDirectory { get; }

        public ConfigFile Config { get; }

        public void Register<T>(T service) where T : class
        {
            if (_services.ContainsKey(typeof(T)))
                throw new InvalidOperationException($"Serviço {typeof(T).Name} já registrado.");
            _services[typeof(T)] = service ?? throw new ArgumentNullException(nameof(service));
        }

        public T Resolve<T>() where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;
            throw new InvalidOperationException(
                $"Serviço {typeof(T).Name} não encontrado. O módulo que o fornece foi registrado antes no JetPlugin?");
        }
    }
}
