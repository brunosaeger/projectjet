using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace JetProject.Framework.Commands
{
    /// <summary>
    /// Serviço. Comandos de desenvolvimento digitados no chat do jogo, com prefixo "." (ex.: ".chestluck 0.5").
    /// Executam no SERVIDOR (quem hospeda a partida); a mensagem do comando não aparece no chat.
    /// </summary>
    internal sealed class ChatCommandRegistry
    {
        public const char Prefix = '.';

        private readonly Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        /// <param name="handler">Recebe o contexto (argumentos + quem digitou) e devolve a resposta exibida no chat.</param>
        public void Register(string name, string usage, Func<ChatCommandContext, string> handler)
        {
            if (_commands.ContainsKey(name))
                throw new InvalidOperationException($"Comando .{name} já registrado.");
            _commands[name] = new Command(usage, handler);
        }

        /// <summary>Tenta tratar o texto como comando. Retorna false se não começar com o prefixo.</summary>
        internal bool TryExecute(string text, GameObject sender, out string reply)
        {
            reply = null;
            if (string.IsNullOrWhiteSpace(text) || text[0] != Prefix)
                return false;

            var parts = text.Substring(1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            if (parts[0].Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                reply = "Comandos: " + string.Join("  |  ", _commands.OrderBy(c => c.Key).Select(c => $".{c.Key} {c.Value.Usage}"));
                return true;
            }

            if (!_commands.TryGetValue(parts[0], out var command))
            {
                reply = $"Comando desconhecido: .{parts[0]} (use .help)";
                return true;
            }

            try
            {
                reply = command.Handler(new ChatCommandContext(parts.Skip(1).ToArray(), sender));
            }
            catch (Exception e)
            {
                reply = $"Erro em .{parts[0]}: {e.Message}. Uso: .{parts[0]} {command.Usage}";
            }
            return true;
        }

        /// <summary>Lê um número aceitando ponto ou vírgula como separador decimal.</summary>
        public static float ParseFloat(string text) =>
            float.Parse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture);

        private readonly struct Command
        {
            public Command(string usage, Func<ChatCommandContext, string> handler)
            {
                Usage = usage;
                Handler = handler;
            }

            public string Usage { get; }
            public Func<ChatCommandContext, string> Handler { get; }
        }
    }
}
