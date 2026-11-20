using JetProject.Framework.Content;
using JetProject.Framework.Effects;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;

namespace JetProject.Characters.Jet.Effects
{
    /// <summary>
    /// Fumaça sob a jetpack quando voando perto do chão: cópia da Smokebomb da Bandit (Bandit2SmokeBomb), menor,
    /// com tremor de tela FRACO (em loop, o tremor original seria contínuo e forte) e sem som (o original não tem som).
    /// Original inspecionado: duração 2 s, applyScale = 0, ShakeEmitter (0,15 s, raio 40), soundName vazio.
    /// </summary>
    internal static class JetpackGroundSmokeEffect
    {
        public static GameObject Prefab { get; private set; }

        public static void Create(ContentRegistry content)
        {
            var effect = EffectFactory.Clone(RoR2_Base_Bandit2.Bandit2SmokeBomb_prefab, "JetJetpackGroundSmoke");
            EffectFactory.ScaleScreenShake(effect, JetConfig.GroundSmokeShakeIntensity.Value);
            EffectFactory.Mute(effect);
            EffectFactory.ScaleHierarchy(effect, JetConfig.GroundSmokeScale.Value);
            Prefab = EffectFactory.Register(content, effect);
        }
    }
}
