using RoR2;
using UnityEngine;

namespace JetProject.Framework.Commands
{
    /// <summary>Dados de uma execução de comando de chat: argumentos e quem digitou.</summary>
    internal sealed class ChatCommandContext
    {
        public ChatCommandContext(string[] args, GameObject senderObject)
        {
            Args = args;
            User = senderObject ? senderObject.GetComponent<NetworkUser>() : null;
        }

        /// <summary>Argumentos após o nome do comando.</summary>
        public string[] Args { get; }

        /// <summary>Jogador que digitou (pode ser null em mensagens sem remetente).</summary>
        public NetworkUser User { get; }

        public CharacterMaster Master => User ? User.master : null;

        public CharacterBody Body => Master ? Master.GetBody() : null;

        public string SenderName => User ? User.userName : "?";

        /// <summary>Argumento inteiro na posição <paramref name="index"/>, ou <paramref name="fallback"/> se ausente.</summary>
        public int IntArg(int index, int fallback) =>
            Args.Length > index ? int.Parse(Args[index]) : fallback;
    }
}
