using JetProject.Characters.Jet.Effects;
using JetProject.Framework.Combat;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// Enquanto a jetpack está ligada e o Jet está perto do chão, solta a fumaça da Smokebomb da Bandit no chão
    /// logo abaixo, em loop rápido. Puramente visual: roda em TODOS os clientes, cada um gera a própria fumaça
    /// (sem tráfego de rede). O estado da jetpack chega aos clientes pela máquina "Jetpack" sincronizada.
    /// </summary>
    public sealed class JetpackGroundSmoke : MonoBehaviour
    {
        private CharacterBody _body;
        private EntityStateMachine _jetpackMachine;
        private EntityStateMachine _bodyMachine;
        private float _timer;

        private void Start()
        {
            _body = GetComponent<CharacterBody>();
            _jetpackMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Jetpack);
            _bodyMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);
        }

        private void Update()
        {
            if (!JetConfig.GroundSmokeEnabled.Value || !JetpackGroundSmokeEffect.Prefab || !_body
                || !JetpackVisualState.IsThrusting(_jetpackMachine, _bodyMachine, out _))
            {
                _timer = 0f;   // ao ligar a jetpack, a primeira fumaça sai na hora
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0f)
                return;
            _timer = JetConfig.GroundSmokeInterval.Value;

            if (!GroundFinder.TryFindBelow(_body.footPosition, JetConfig.GroundSmokeMaxHeight.Value, out var ground, out var normal))
                return;

            EffectManager.SpawnEffect(JetpackGroundSmokeEffect.Prefab, new EffectData
            {
                origin = ground,
                rotation = Quaternion.FromToRotation(Vector3.up, normal),
            }, transmit: false);
        }
    }
}
