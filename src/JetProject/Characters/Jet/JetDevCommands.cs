using JetProject.Framework.Commands;
using RoR2;

namespace JetProject.Characters.Jet
{
    /// <summary>
    /// Comandos de chat para testes do Jet (ver Framework/Commands).
    /// Os de chance alteram o config ao vivo (o valor é salvo no .cfg); os de item dão itens a quem digitou.
    /// </summary>
    internal static class JetDevCommands
    {
        public static void Register(ChatCommandRegistry commands)
        {
            commands.Register("chestluck", "<porcentagem>", context =>
            {
                if (context.Args.Length == 0)
                    return $"Chance de baú: {JetConfig.SecondaryChestChancePercent.Value}%";
                JetConfig.SecondaryChestChancePercent.Value = ChatCommandRegistry.ParseFloat(context.Args[0]);
                return $"Chance de baú agora é {JetConfig.SecondaryChestChancePercent.Value}%";
            });

            commands.Register("legenchestluck", "<porcentagem>", context =>
            {
                if (context.Args.Length == 0)
                    return $"Chance de baú lendário: {JetConfig.SecondaryLegendaryChestChancePercent.Value}%";
                JetConfig.SecondaryLegendaryChestChancePercent.Value = ChatCommandRegistry.ParseFloat(context.Args[0]);
                return $"Chance de baú lendário agora é {JetConfig.SecondaryLegendaryChestChancePercent.Value}%";
            });

            // Itens que interagem com o kit do Jet (combustível e velocidade de voo).
            ItemCommands.RegisterGive(commands, "givegas", "Gasoline", () => RoR2Content.Items.IgniteOnKill);
            ItemCommands.RegisterGive(commands, "givehopoo", "Hopoo Feather", () => RoR2Content.Items.Feather);
            ItemCommands.RegisterGive(commands, "giveenergy", "Energy Drink", () => RoR2Content.Items.SprintBonus);
            ItemCommands.RegisterGive(commands, "givegoat", "Paul's Goat Hoof", () => RoR2Content.Items.Hoof);
        }
    }
}
