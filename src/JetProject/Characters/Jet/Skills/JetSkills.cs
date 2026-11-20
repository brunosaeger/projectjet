using EntityStates;
using JetProject.Characters.Jet.States.Movement;
using JetProject.Characters.Jet.States.Utility;
using JetProject.Characters.Jet.States.Weapon;
using JetProject.Framework.Content;
using JetProject.Framework.Skills;
using RoR2.Skills;

namespace JetProject.Characters.Jet.Skills
{
    /// <summary>SkillDefs e famílias do Jet. Cada skill nova ganha um método Create* aqui.</summary>
    internal static class JetSkills
    {
        public static SkillFamily CreatePrimaryFamily(ContentRegistry content)
        {
            var handCannon = SkillFactory.CreateSkill<SteppedSkillDef>("JetHandCannon", typeof(FireHandCannon), JetStateMachines.Weapon, def =>
            {
                def.skillNameToken = JetTokens.PrimaryName;
                def.skillDescriptionToken = JetTokens.PrimaryDescription;
                def.stepCount = FireHandCannon.StepCount;
                def.stepGraceDuration = JetConfig.PrimaryComboResetTime.Value;
                def.baseRechargeInterval = 0f;
                def.baseMaxStock = 1;
                def.rechargeStock = 1;
                def.requiredStock = 1;
                def.stockToConsume = 1;
                def.interruptPriority = InterruptPriority.Any;
                def.isCombatSkill = true;
                def.cancelSprintingOnActivation = true;
                def.mustKeyPress = false;
            });

            var family = SkillFactory.CreateFamily("JetPrimaryFamily", handCannon);

            content.AddSkill(handCannon);
            content.AddSkillFamily(family);
            content.AddEntityState<FireHandCannon>();
            return family;
        }

        public static SkillFamily CreateSecondaryFamily(ContentRegistry content)
        {
            // Ativa o SupplyFlareEntry, que escolhe disparo imediato (chão) ou mira (ar).
            var supplyFlare = SkillFactory.CreateSkill<SkillDef>("JetSupplyFlare", typeof(SupplyFlareEntry), JetStateMachines.Weapon, def =>
            {
                def.skillNameToken = JetTokens.SecondaryName;
                def.skillDescriptionToken = JetTokens.SecondaryDescription;
                def.baseRechargeInterval = JetConfig.SecondaryCooldown.Value;
                def.baseMaxStock = 1;
                def.rechargeStock = 1;
                def.requiredStock = 1;
                def.stockToConsume = 1;
                def.interruptPriority = InterruptPriority.Skill;
                def.isCombatSkill = true;
                def.cancelSprintingOnActivation = true;
                def.mustKeyPress = true;
            });

            var family = SkillFactory.CreateFamily("JetSecondaryFamily", supplyFlare);

            content.AddSkill(supplyFlare);
            content.AddSkillFamily(family);
            content.AddEntityState<SupplyFlareEntry>();
            content.AddEntityState<FireSupplyFlare>();
            content.AddEntityState<AimSupplyFlare>();
            content.AddEntityState<JetDodge>();
            return family;
        }

        public static SkillFamily CreateUtilityFamily(ContentRegistry content)
        {
            // Só no ar. Ativa o BarrelRollEntry na máquina Body (rolagens e Ace substituem o movimento normal).
            var barrelRoll = SkillFactory.CreateSkill<AirborneSkillDef>("JetBarrelRoll", typeof(BarrelRollEntry), JetStateMachines.Body, def =>
            {
                def.skillNameToken = JetTokens.UtilityName;
                def.skillDescriptionToken = JetTokens.UtilityDescription;
                def.baseMaxStock = JetConfig.UtilityCharges.Value;
                def.baseRechargeInterval = JetConfig.UtilityCooldown.Value;
                def.rechargeStock = 1;
                def.requiredStock = 1;
                def.stockToConsume = 1;
                def.fullRestockOnAssign = true;
                def.interruptPriority = InterruptPriority.PrioritySkill;
                def.isCombatSkill = true;
                def.cancelSprintingOnActivation = false;
                def.mustKeyPress = true;
            });

            var family = SkillFactory.CreateFamily("JetUtilityFamily", barrelRoll);

            content.AddSkill(barrelRoll);
            content.AddSkillFamily(family);
            content.AddEntityState<BarrelRollEntry>();
            content.AddEntityState<AceTheSkies>();
            content.AddEntityState<AceTheSkiesBurn>();
            return family;
        }
    }
}
