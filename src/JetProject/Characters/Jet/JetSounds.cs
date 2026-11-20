namespace JetProject.Characters.Jet
{
    /// <summary>
    /// Nomes dos eventos Wwise do Jet (soundbank JetProject). Cada som de wwise/sounds.json vira "Play_&lt;nome&gt;".
    /// Tocar com <c>Util.PlaySound(JetSounds.X, gameObject)</c> em TODOS os clientes (ex.: OnEnter do estado).
    /// </summary>
    internal static class JetSounds
    {
        public const string HandCannonFire = "Play_Jet_HandCannon_Fire";
        public const string HandCannonFirePiercing = "Play_Jet_HandCannon_FirePiercing";

        /// <summary>Toda vez que a jetpack liga.</summary>
        public const string JetpackIgnite = "Play_Jet_Jetpack_Ignite";

        /// <summary>Loop enquanto a jetpack voa (parar com <see cref="JetpackLoopStop"/>).</summary>
        public const string JetpackLoop = "Play_Jet_Jetpack_Loop";
        public const string JetpackLoopStop = "Stop_Jet_Jetpack_Loop";

        /// <summary>Aviso de combustível acabando (só para o próprio jogador).</summary>
        public const string JetpackLowFuel = "Play_Jet_Jetpack_LowFuel";

        // Eventos VANILLA reaproveitados (estão nos soundbanks do jogo).

        /// <summary>Disparo do sinalizador: som de lançamento dos fogos de artifício (Bundle of Fireworks).</summary>
        public const string SupplyFlareFire = "Play_item_proc_firework_fire";

        /// <summary>Feixe de entrega aparecendo: aviso antes do impacto do Orbital Probe do Captain.</summary>
        public const string SupplyBeaconWarning = "Play_captain_shift_preImpact";

        /// <summary>Impacto do Orbital Probe do Captain (o efeito de impacto dele NÃO traz som; é este evento).</summary>
        public const string SupplyBeaconImpact = "Play_captain_shift_impact";

        /// <summary>Baú caindo no fim do feixe: impacto da entrega de suprimentos do Captain.</summary>
        public const string SupplyDropLanding = "Play_captain_R_impact";
    }
}
