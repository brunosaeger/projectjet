namespace JetProject.Characters.Jet
{
    /// <summary>Tokens de idioma do Jet. Textos ficam em Resources/Language/&lt;idioma&gt;/Jet.json.</summary>
    internal static class JetTokens
    {
        private const string Prefix = PluginInfo.TokenPrefix + "JET_";

        public const string Name = Prefix + "NAME";
        public const string Subtitle = Prefix + "SUBTITLE";
        public const string Description = Prefix + "DESCRIPTION";
        public const string OutroFlavor = Prefix + "OUTRO_FLAVOR";
        public const string OutroFailure = Prefix + "OUTRO_FAILURE";

        public const string PassiveName = Prefix + "PASSIVE_NAME";
        public const string PassiveDescription = Prefix + "PASSIVE_DESCRIPTION";

        public const string PrimaryName = Prefix + "PRIMARY_NAME";
        public const string PrimaryDescription = Prefix + "PRIMARY_DESCRIPTION";

        public const string SecondaryName = Prefix + "SECONDARY_NAME";
        public const string SecondaryDescription = Prefix + "SECONDARY_DESCRIPTION";
        public const string SupplyDropMessage = Prefix + "SUPPLY_DROP_MESSAGE";

        public const string UtilityName = Prefix + "UTILITY_NAME";
        public const string UtilityDescription = Prefix + "UTILITY_DESCRIPTION";
    }
}
