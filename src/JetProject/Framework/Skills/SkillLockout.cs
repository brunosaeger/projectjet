using RoR2;

namespace JetProject.Framework.Skills
{
    /// <summary>Penalidades de recarga aplicadas a um slot de skill.</summary>
    internal static class SkillLockout
    {
        /// <summary>
        /// Zera todas as cargas e faz a PRIMEIRA voltar só depois de <paramref name="seconds"/>
        /// (as seguintes no ritmo normal). Usa o cronômetro nativo da recarga, então o ícone mostra o tempo certo:
        /// recarga restante = intervalo − cronômetro, logo cronômetro = intervalo − segundos.
        /// Chamar na autoridade (o estado das skills é do dono).
        /// </summary>
        public static void EmptyAndDelay(GenericSkill slot, float seconds)
        {
            if (!slot)
                return;
            slot.RemoveAllStocks();
            slot.rechargeStopwatch = slot.finalRechargeInterval - seconds;
        }
    }
}
