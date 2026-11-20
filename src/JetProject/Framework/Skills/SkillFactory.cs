using System;
using System.Linq;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace JetProject.Framework.Skills
{
    /// <summary>Criação de SkillDefs/SkillFamilies e atribuição a slots de um body.</summary>
    internal static class SkillFactory
    {
        /// <summary>
        /// Cria um SkillDef (ou subclasse, ex.: SteppedSkillDef) já apontando para o estado e a máquina de ativação.
        /// Os demais campos são definidos em <paramref name="configure"/>.
        /// </summary>
        public static T CreateSkill<T>(string skillName, Type activationState, string stateMachine, Action<T> configure = null)
            where T : SkillDef
        {
            var def = ScriptableObject.CreateInstance<T>();
            ((ScriptableObject)def).name = skillName;
            def.skillName = skillName;
            def.activationState = new SerializableEntityStateType(activationState);
            def.activationStateMachineName = stateMachine;
            configure?.Invoke(def);
            return def;
        }

        public static SkillFamily CreateFamily(string familyName, params SkillDef[] variants)
        {
            var family = ScriptableObject.CreateInstance<SkillFamily>();
            ((ScriptableObject)family).name = familyName;
            family.variants = variants.Select(def => new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false),
            }).ToArray();
            return family;
        }

        /// <summary>Substitui a família de um slot (GenericSkill) do prefab.</summary>
        public static void AssignFamily(GenericSkill slot, SkillFamily family)
        {
            if (!slot)
                throw new ArgumentNullException(nameof(slot));
            slot._skillFamily = family;
        }
    }
}
