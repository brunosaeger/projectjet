using RoR2;
using UnityEngine;

namespace JetProject.Framework.Survivors
{
    /// <summary>Dados para criar um SurvivorDef (aparece na tela de seleção).</summary>
    internal struct SurvivorInfo
    {
        public string Name;
        public GameObject BodyPrefab;
        public GameObject DisplayPrefab;
        public string NameToken;
        public string DescriptionToken;
        public string OutroFlavorToken;
        public string OutroFailureToken;
        public Color PrimaryColor;
        public float SortPosition;
    }

    internal static class SurvivorFactory
    {
        public static SurvivorDef Create(SurvivorInfo info)
        {
            var def = ScriptableObject.CreateInstance<SurvivorDef>();
            def.cachedName = info.Name;
            def.bodyPrefab = info.BodyPrefab;
            def.displayPrefab = info.DisplayPrefab;
            def.displayNameToken = info.NameToken;
            def.descriptionToken = info.DescriptionToken;
            def.outroFlavorToken = info.OutroFlavorToken;
            def.mainEndingEscapeFailureFlavorToken = info.OutroFailureToken;
            def.primaryColor = info.PrimaryColor;
            def.desiredSortPosition = info.SortPosition;
            return def;
        }
    }
}
