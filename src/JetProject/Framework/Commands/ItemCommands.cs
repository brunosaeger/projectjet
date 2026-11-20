using System;
using RoR2;

namespace JetProject.Framework.Commands
{
    /// <summary>Helpers para comandos de teste que dão/retiram itens de quem digitou.</summary>
    internal static class ItemCommands
    {
        /// <summary>
        /// Registra ".{nome} [quantidade]": dá o item a quem digitou (padrão 1; negativo retira).
        /// <paramref name="item"/> é resolvido na hora do comando: os ItemDefs do jogo só existem depois que o
        /// conteúdo carrega, depois do registro dos comandos.
        /// </summary>
        public static void RegisterGive(ChatCommandRegistry commands, string name, string displayName, Func<ItemDef> item)
        {
            commands.Register(name, "[quantidade]", context =>
            {
                var itemDef = item();
                var inventory = context.Master ? context.Master.inventory : null;
                if (!itemDef || !inventory)
                    return $"Não foi possível dar {displayName}: sem personagem/inventário (está numa partida?).";

                int amount = context.IntArg(0, 1);
                if (amount >= 0)
                    inventory.GiveItemPermanent(itemDef, amount);
                else
                    inventory.RemoveItemPermanent(itemDef.itemIndex, Math.Min(-amount, inventory.GetItemCountPermanent(itemDef)));

                return $"{context.SenderName}: {(amount >= 0 ? "+" : "")}{amount} {displayName} (total: {inventory.GetItemCountPermanent(itemDef)})";
            });
        }
    }
}
