using JetProject.Framework.Effects;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;

namespace JetProject.Characters.Jet.Effects
{
    /// <summary>
    /// PLACEHOLDER dos propulsores da jetpack, a partir dos jatos do Tactical Slide/Dive do Commando (CommandoDashJets).
    /// O original é dividido em duas peças para a chama poder piscar:
    ///  - <see cref="Jets"/>: tudo menos a chama (Jet, Ring, Distortion, Sparks + luz), em loop regular;
    ///  - <see cref="Flame"/>: só a chama grande vermelha ("Flare"), em ritmo irregular e tamanho aleatório.
    /// Cada peça tem uma variante AZUL (queima do Ace the Skies).
    /// Original inspecionado (tools/assets/inspect-particles.ps1): Flare = maior partícula (tamanho 1.0), cor (1, 0.49, 0.31),
    /// vida 0,1 s. Efeito dura 0,3 s, sem som, micro-tremor de 0,05 s. Instanciado direto (sem EffectCatalog), como no vanilla.
    /// </summary>
    internal static class JetpackThrusterEffect
    {
        private const string FlameChild = "Flare";
        private static readonly Color BlueFlameColor = new Color(0.35f, 0.65f, 1f);

        public static GameObject Jets { get; private set; }
        public static GameObject Flame { get; private set; }
        public static GameObject JetsBlue { get; private set; }
        public static GameObject FlameBlue { get; private set; }

        public static void Create()
        {
            Jets = CreateJets("JetJetpackThrusterJets");
            Flame = CreateFlame("JetJetpackThrusterFlame");

            JetsBlue = CreateJets("JetJetpackThrusterJetsBlue");
            EffectFactory.TintParticles(JetsBlue, BlueFlameColor);
            FlameBlue = CreateFlame("JetJetpackThrusterFlameBlue");
            EffectFactory.TintParticles(FlameBlue, BlueFlameColor);
        }

        private static GameObject CreateJets(string name)
        {
            var jets = EffectFactory.Clone(RoR2_Base_Commando.CommandoDashJets_prefab, name);
            EffectFactory.RemoveChildren(jets, FlameChild);
            EffectFactory.ScaleScreenShake(jets, JetConfig.ThrusterShakeIntensity.Value);
            return jets;
        }

        private static GameObject CreateFlame(string name)
        {
            var flame = EffectFactory.Clone(RoR2_Base_Commando.CommandoDashJets_prefab, name);
            EffectFactory.KeepOnlyChildren(flame, FlameChild);
            EffectFactory.RemoveLights(flame);
            EffectFactory.RemoveScreenShake(flame);
            EffectFactory.UseHierarchyScaling(flame);
            return flame;
        }
    }
}
