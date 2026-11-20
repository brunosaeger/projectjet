using JetProject.Framework.Assets;
using JetProject.Framework.Content;
using R2API;
using RoR2;
using UnityEngine;

namespace JetProject.Framework.Effects
{
    /// <summary>
    /// Cópias modificadas de efeitos visuais do jogo (sem rede) registradas no EffectCatalog,
    /// para serem usadas com EffectManager.SpawnEffect.
    /// </summary>
    internal static class EffectFactory
    {
        /// <summary>Clona um efeito do jogo (apenas visual, sem registro de rede).</summary>
        public static GameObject Clone(string addressableKey, string newName)
        {
            var original = GameAssets.Load<GameObject>(addressableKey);
            return original.InstantiateClone(newName, registerNetwork: false);
        }

        /// <summary>Remove o tremor de tela (ShakeEmitter) do efeito e dos filhos.</summary>
        public static void RemoveScreenShake(GameObject effect)
        {
            foreach (var shake in effect.GetComponentsInChildren<ShakeEmitter>(true))
                Object.DestroyImmediate(shake);
        }

        /// <summary>
        /// Multiplica a intensidade (amplitude) do tremor de tela. Útil em efeitos repetidos em loop, onde o tremor
        /// original viraria um tremor contínuo forte. Fator ≤ 0 remove o tremor.
        /// </summary>
        public static void ScaleScreenShake(GameObject effect, float factor)
        {
            if (factor <= 0f)
            {
                RemoveScreenShake(effect);
                return;
            }
            foreach (var shake in effect.GetComponentsInChildren<ShakeEmitter>(true))
            {
                var wave = shake.wave;
                wave.amplitude *= factor;
                shake.wave = wave;
            }
        }

        /// <summary>Remove sons embutidos (EffectComponent.soundName).</summary>
        public static void Mute(GameObject effect)
        {
            foreach (var component in effect.GetComponentsInChildren<EffectComponent>(true))
                component.soundName = "";
        }

        /// <summary>
        /// Escala o efeito inteiro. Partículas só seguem a escala do pai no modo Hierarchy, então todos os
        /// ParticleSystems passam para esse modo. Necessário quando o efeito ignora EffectData.scale (applyScale = 0).
        /// </summary>
        public static void ScaleHierarchy(GameObject effect, float scale)
        {
            if (Mathf.Approximately(scale, 1f))
                return;
            UseHierarchyScaling(effect);
            effect.transform.localScale *= scale;
        }

        /// <summary>Remove os filhos diretos com estes nomes (ex.: separar uma parte do efeito para outro uso).</summary>
        public static void RemoveChildren(GameObject effect, params string[] childNames)
        {
            foreach (var name in childNames)
            {
                var child = effect.transform.Find(name);
                if (child)
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>Mantém só os filhos diretos com estes nomes; remove todos os outros.</summary>
        public static void KeepOnlyChildren(GameObject effect, params string[] childNames)
        {
            for (int i = effect.transform.childCount - 1; i >= 0; i--)
            {
                var child = effect.transform.GetChild(i);
                if (System.Array.IndexOf(childNames, child.name) < 0)
                    Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>Remove luzes (Light e o animador de intensidade) do efeito inteiro.</summary>
        public static void RemoveLights(GameObject effect)
        {
            foreach (var curve in effect.GetComponentsInChildren<LightIntensityCurve>(true))
                Object.DestroyImmediate(curve);
            foreach (var light in effect.GetComponentsInChildren<Light>(true))
                Object.DestroyImmediate(light);
        }

        /// <summary>
        /// Faz as partículas seguirem a escala do transform (modo Hierarchy). Necessário para variar o tamanho de cada
        /// instância em runtime via transform.localScale.
        /// </summary>
        public static void UseHierarchyScaling(GameObject effect)
        {
            foreach (var particles in effect.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = particles.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }
        }

        /// <summary>
        /// Muda a cor inicial de todas as partículas (mantendo o alfa de cada uma). O resultado final ainda depende do
        /// material/textura; em efeitos de fogo com shaders do jogo, a cor de vértice tinge a partícula.
        /// </summary>
        public static void TintParticles(GameObject effect, Color color)
        {
            foreach (var particles in effect.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = particles.main;
                float alpha = main.startColor.color.a;
                main.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, alpha));
            }
            foreach (var light in effect.GetComponentsInChildren<Light>(true))
                light.color = color;
        }

        /// <summary>Registra o efeito no conteúdo do mod (obrigatório para EffectManager.SpawnEffect).</summary>
        public static GameObject Register(ContentRegistry content, GameObject effect)
        {
            content.AddEffect(new EffectDef(effect));
            return effect;
        }
    }
}
