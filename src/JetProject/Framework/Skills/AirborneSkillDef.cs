using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;

namespace JetProject.Framework.Skills
{
    /// <summary>SkillDef que só fica pronta com o personagem NO AR (no chão aparece como indisponível).</summary>
    public class AirborneSkillDef : SkillDef
    {
        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            var motor = skillSlot.characterBody ? skillSlot.characterBody.characterMotor : null;
            return base.IsReady(skillSlot) && motor && !motor.isGrounded;
        }
    }
}
