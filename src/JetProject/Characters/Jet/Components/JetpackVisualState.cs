using JetProject.Characters.Jet.States;
using JetProject.Characters.Jet.States.Jetpack;
using RoR2;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>Pergunta única dos visuais da jetpack: ela está propulsionando agora? Com chama azul?</summary>
    internal static class JetpackVisualState
    {
        public static bool IsThrusting(EntityStateMachine jetpackMachine, EntityStateMachine bodyMachine, out bool blueFlame)
        {
            if (bodyMachine && bodyMachine.state is IJetpackThrustVisual visual)
            {
                blueFlame = visual.BlueFlame;
                return true;
            }

            blueFlame = false;
            return JetpackFlight.IsActive(jetpackMachine);
        }
    }
}
