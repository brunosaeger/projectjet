namespace JetProject.Characters.Jet.Systems
{
    /// <summary>
    /// Um disparo da secundária. Compartilhado pelo sinalizador, pela zona de fogo que ele cria e pelas queimaduras
    /// que eles aplicam, e sobrevive à destruição do projétil. Garante UM sorteio de baú por disparo.
    /// Só existe no servidor.
    /// </summary>
    internal sealed class SupplyShot
    {
        /// <summary>A chance de baú deste disparo já foi usada (por qualquer morte: direto, área, zona ou queimadura).</summary>
        public bool ChestRolled { get; set; }
    }
}
