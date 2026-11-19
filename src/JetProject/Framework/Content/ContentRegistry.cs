using System;
using System.Collections.Generic;
using RoR2;
using RoR2.ContentManagement;
using RoR2.Skills;
using UnityEngine;

namespace JetProject.Framework.Content
{
    /// <summary>
    /// Ponto único de registro de conteúdo do jogo. Módulos de conteúdo adicionam aqui;
    /// o <see cref="JetContentPackProvider"/> despeja tudo no ContentPack quando o jogo pede.
    /// Registrar depois do carregamento do jogo é erro, pois o ContentPack já foi selado.
    /// </summary>
    internal sealed class ContentRegistry
    {
        private readonly List<GameObject> _bodyPrefabs = new List<GameObject>();
        private readonly List<GameObject> _masterPrefabs = new List<GameObject>();
        private readonly List<GameObject> _projectilePrefabs = new List<GameObject>();
        private readonly List<GameObject> _networkedObjectPrefabs = new List<GameObject>();
        private readonly List<SurvivorDef> _survivorDefs = new List<SurvivorDef>();
        private readonly List<SkillDef> _skillDefs = new List<SkillDef>();
        private readonly List<SkillFamily> _skillFamilies = new List<SkillFamily>();
        private readonly List<Type> _entityStateTypes = new List<Type>();
        private readonly List<BuffDef> _buffDefs = new List<BuffDef>();
        private readonly List<EffectDef> _effectDefs = new List<EffectDef>();
        private readonly List<NetworkSoundEventDef> _networkSoundEventDefs = new List<NetworkSoundEventDef>();

        public bool IsSealed { get; private set; }

        public void AddBody(GameObject prefab) => Add(_bodyPrefabs, prefab);
        public void AddMaster(GameObject prefab) => Add(_masterPrefabs, prefab);
        public void AddProjectile(GameObject prefab) => Add(_projectilePrefabs, prefab);
        public void AddNetworkedObject(GameObject prefab) => Add(_networkedObjectPrefabs, prefab);
        public void AddSurvivor(SurvivorDef def) => Add(_survivorDefs, def);
        public void AddSkill(SkillDef def) => Add(_skillDefs, def);
        public void AddSkillFamily(SkillFamily family) => Add(_skillFamilies, family);
        public void AddEntityState(Type stateType) => Add(_entityStateTypes, stateType);
        public void AddBuff(BuffDef def) => Add(_buffDefs, def);
        public void AddEffect(EffectDef def) => Add(_effectDefs, def);
        public void AddNetworkSound(NetworkSoundEventDef def) => Add(_networkSoundEventDefs, def);

        public void AddEntityState<TState>() where TState : EntityStates.EntityState => AddEntityState(typeof(TState));

        /// <summary>Copia o conteúdo para o ContentPack e sela o registro.</summary>
        internal void PopulateAndSeal(ContentPack pack)
        {
            pack.bodyPrefabs.Add(_bodyPrefabs.ToArray());
            pack.masterPrefabs.Add(_masterPrefabs.ToArray());
            pack.projectilePrefabs.Add(_projectilePrefabs.ToArray());
            pack.networkedObjectPrefabs.Add(_networkedObjectPrefabs.ToArray());
            pack.survivorDefs.Add(_survivorDefs.ToArray());
            pack.skillDefs.Add(_skillDefs.ToArray());
            pack.skillFamilies.Add(_skillFamilies.ToArray());
            pack.entityStateTypes.Add(_entityStateTypes.ToArray());
            pack.buffDefs.Add(_buffDefs.ToArray());
            pack.effectDefs.Add(_effectDefs.ToArray());
            pack.networkSoundEventDefs.Add(_networkSoundEventDefs.ToArray());
            IsSealed = true;
        }

        private void Add<T>(List<T> list, T item)
        {
            if (IsSealed)
                throw new InvalidOperationException($"ContentRegistry já foi selado; não é possível adicionar {typeof(T).Name}.");
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (!list.Contains(item))
                list.Add(item);
        }
    }
}
