using HarmonyLib;
using JetProject.Framework.Core;
using RoR2;

namespace JetProject.Framework.Commands
{
    /// <summary>
    /// Registra o <see cref="ChatCommandRegistry"/> e intercepta, no servidor, mensagens de chat que começam com ".".
    /// Ponto de interceptação: Chat.SendBroadcastChat(ChatMessageBase, int), por onde passa toda mensagem de jogador
    /// (o comando "say" do cliente chega ao servidor e é retransmitido por aqui).
    /// </summary>
    internal sealed class ChatCommandsModule : IModule
    {
        private static ChatCommandRegistry _registry;

        public string Name => "ChatCommands";

        public void Initialize(ModContext context)
        {
            _registry = new ChatCommandRegistry();
            context.Register(_registry);

            var harmony = new Harmony(PluginInfo.Guid + ".chatcommands");
            var target = AccessTools.Method(typeof(Chat), nameof(Chat.SendBroadcastChat),
                new[] { typeof(ChatMessageBase), typeof(int) });
            harmony.Patch(target, prefix: new HarmonyMethod(typeof(ChatCommandsModule), nameof(BeforeBroadcast)));
        }

        /// <returns>false = engole a mensagem original (o comando não aparece no chat).</returns>
        private static bool BeforeBroadcast(ChatMessageBase message)
        {
            if (!(message is Chat.UserChatMessage userMessage) || _registry == null)
                return true;

            if (!_registry.TryExecute(userMessage.text, userMessage.sender, out var reply))
                return true;

            if (!string.IsNullOrEmpty(reply))
            {
                // SimpleChatMessage não é UserChatMessage, então não reentra no tratamento de comando.
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>[{PluginInfo.Name}]</style> {reply}" });
            }
            return false;
        }
    }
}
