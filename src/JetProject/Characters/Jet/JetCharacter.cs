using EntityStates;
using JetProject.Characters.Jet.Components;
using JetProject.Characters.Jet.Projectiles;
using JetProject.Characters.Jet.Systems;
using JetProject.Characters.Jet.Skills;
using JetProject.Characters.Jet.States;
using JetProject.Characters.Jet.States.Jetpack;
using JetProject.Framework.Bodies;
using JetProject.Framework.Commands;
using JetProject.Framework.Content;
using JetProject.Framework.Core;
using JetProject.Framework.Skills;
using JetProject.Framework.Survivors;
using JetProject.Framework.UI;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;

namespace JetProject.Characters.Jet
{
    /// <summary>
    /// Monta o Jet: body, máquinas de estado, componentes, skills e SurvivorDef.
    /// PROVISÓRIO: o body e o display são clones do Commando até o modelo próprio existir (ver art/ e unity/).
    /// </summary>
    internal sealed class JetCharacter : ICharacter
    {
        private static readonly string PlaceholderBodyKey = RoR2_Base_Commando.CommandoBody_prefab;
        private static readonly string PlaceholderDisplayKey = RoR2_Base_Commando.CommandoDisplay_prefab;

        public static readonly Color BodyColor = new Color(0.86f, 0.62f, 0.12f);

        public string Name => "Jet";

        public void Register(ModContext context)
        {
            JetConfig.Bind(context.Config);
            JetAssets.Load();
            var content = context.Resolve<ContentRegistry>();

            SupplyFireZoneProjectile.Create(content);
            SupplyFlareProjectile.Create(content);
            SupplyBeaconProjectile.Create(content);
            SupplyDropSystem.Initialize();
            JetDevCommands.Register(context.Resolve<ChatCommandRegistry>());

            var body = CreateBody(content);
            var display = BodyFactory.CloneDisplay(PlaceholderDisplayKey, "JetDisplay");

            var survivor = SurvivorFactory.Create(new SurvivorInfo
            {
                Name = "Jet",
                BodyPrefab = body,
                DisplayPrefab = display,
                NameToken = JetTokens.Name,
                DescriptionToken = JetTokens.Description,
                OutroFlavorToken = JetTokens.OutroFlavor,
                OutroFailureToken = JetTokens.OutroFailure,
                PrimaryColor = BodyColor,
                SortPosition = 100f,
            });

            content.AddBody(body);
            content.AddSurvivor(survivor);
        }

        private static GameObject CreateBody(ContentRegistry content)
        {
            var bodyObject = BodyFactory.CloneBody(PlaceholderBodyKey, "JetBody");

            var body = bodyObject.GetComponent<CharacterBody>();
            body.baseNameToken = JetTokens.Name;
            body.subtitleNameToken = JetTokens.Subtitle;
            body.bodyColor = BodyColor;
            body.baseJumpCount = 1;

            // Passiva Jetpack
            bodyObject.AddComponent<JetpackFuel>();
            bodyObject.AddComponent<ResourceMeterHud>();
            StateMachineSetup.SetMainState(bodyObject, JetStateMachines.Body, typeof(JetCharacterMain));
            StateMachineSetup.Add(bodyObject, JetStateMachines.Jetpack, typeof(Idle), typeof(Idle));
            content.AddEntityState<JetCharacterMain>();
            content.AddEntityState<JetpackFlight>();

            var skillLocator = bodyObject.GetComponent<SkillLocator>();
            skillLocator.passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = JetTokens.PassiveName,
                skillDescriptionToken = JetTokens.PassiveDescription,
            };

            // Skills próprias: primária e secundária. Utilitária/especial ainda são as do Commando (provisório).
            SkillFactory.AssignFamily(skillLocator.primary, JetSkills.CreatePrimaryFamily(content));
            SkillFactory.AssignFamily(skillLocator.secondary, JetSkills.CreateSecondaryFamily(content));

            return bodyObject;
        }
    }
}
