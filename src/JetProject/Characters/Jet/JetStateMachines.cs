namespace JetProject.Characters.Jet
{
    /// <summary>Nomes das EntityStateMachines usadas pelo Jet.</summary>
    internal static class JetStateMachines
    {
        public const string Body = "Body";
        public const string Weapon = "Weapon";

        /// <summary>Máquina própria da jetpack: separada de Body e Weapon para voar e atirar ao mesmo tempo.</summary>
        public const string Jetpack = "Jetpack";
    }
}
