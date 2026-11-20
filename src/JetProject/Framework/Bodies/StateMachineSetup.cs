using System;
using System.Linq;
using EntityStates;
using RoR2;
using UnityEngine;

namespace JetProject.Framework.Bodies
{
    /// <summary>
    /// Configuração de EntityStateMachines em prefabs de body, cuidando das integrações que o jogo exige
    /// (sincronização de rede, reset na morte e em stuns).
    /// </summary>
    internal static class StateMachineSetup
    {
        /// <summary>Troca o estado principal de uma máquina existente (ex.: "Body" → estado de movimento próprio).</summary>
        public static void SetMainState(GameObject body, string machineName, Type mainState)
        {
            var machine = Find(body, machineName);
            machine.mainStateType = new SerializableEntityStateType(mainState);
        }

        /// <summary>
        /// Adiciona uma nova máquina ao body, registrada no NetworkStateMachine (estado sincronizado em rede),
        /// e opcionalmente resetada para o estado idle ao morrer ou ser atordoado.
        /// </summary>
        public static EntityStateMachine Add(GameObject body, string machineName, Type initialState, Type mainState,
            bool resetOnDeathAndStun = true)
        {
            if (body.GetComponents<EntityStateMachine>().Any(m => m.customName == machineName))
                throw new InvalidOperationException($"Body '{body.name}' já possui a máquina '{machineName}'.");

            var machine = body.AddComponent<EntityStateMachine>();
            machine.customName = machineName;
            machine.initialStateType = new SerializableEntityStateType(initialState);
            machine.mainStateType = new SerializableEntityStateType(mainState);

            var network = body.GetComponent<NetworkStateMachine>();
            if (network)
                network.stateMachines = network.stateMachines.Append(machine).ToArray();

            if (resetOnDeathAndStun)
            {
                var death = body.GetComponent<CharacterDeathBehavior>();
                if (death)
                    death.idleStateMachine = death.idleStateMachine.Append(machine).ToArray();

                var hurt = body.GetComponent<SetStateOnHurt>();
                if (hurt)
                    hurt.idleStateMachine = hurt.idleStateMachine.Append(machine).ToArray();
            }

            return machine;
        }

        private static EntityStateMachine Find(GameObject body, string machineName)
        {
            var machine = body.GetComponents<EntityStateMachine>().FirstOrDefault(m => m.customName == machineName);
            if (!machine)
                throw new InvalidOperationException($"Body '{body.name}' não possui a máquina '{machineName}'.");
            return machine;
        }
    }
}
