using JetProject.Characters.Jet.Effects;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// Enquanto a jetpack propulsiona (voo normal ou estados como o Ace the Skies), solta os propulsores
    /// (placeholder: jatos do Commando) nos pontos "LeftJet" e "RightJet" do modelo. Duas camadas:
    ///  - jatos: loop regular e rápido;
    ///  - chama: intervalo e tamanho ALEATÓRIOS, com vida curta (0,1 s), então ela some entre um disparo e outro e pisca.
    /// Variante AZUL quando o estado pede (queima do Ace the Skies).
    /// Os efeitos ficam presos aos pontos (acompanham o personagem). Puramente visual: roda em TODOS os clientes.
    /// Quando o modelo próprio existir, os pontos de propulsão dele devem usar os mesmos nomes (ou ajustar aqui).
    /// </summary>
    public sealed class JetpackThrusters : MonoBehaviour
    {
        private static readonly string[] ThrusterPoints = { "LeftJet", "RightJet" };

        private EntityStateMachine _jetpackMachine;
        private EntityStateMachine _bodyMachine;
        private ModelLocator _modelLocator;
        private ChildLocator _childLocator;
        private float _jetsTimer;
        private float _flameTimer;

        private void Start()
        {
            _jetpackMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Jetpack);
            _bodyMachine = EntityStateMachine.FindByCustomName(gameObject, JetStateMachines.Body);
            _modelLocator = GetComponent<ModelLocator>();
        }

        private void Update()
        {
            if (!JetConfig.ThrustersEnabled.Value || !JetpackVisualState.IsThrusting(_jetpackMachine, _bodyMachine, out bool blue))
            {
                _jetsTimer = 0f;
                _flameTimer = 0f;
                return;
            }

            var childLocator = GetChildLocator();
            if (!childLocator)
                return;

            _jetsTimer -= Time.deltaTime;
            if (_jetsTimer <= 0f)
            {
                _jetsTimer = JetConfig.ThrusterInterval.Value;
                SpawnAtPoints(childLocator, blue ? JetpackThrusterEffect.JetsBlue : JetpackThrusterEffect.Jets, 1f);
            }

            _flameTimer -= Time.deltaTime;
            if (_flameTimer <= 0f)
            {
                _flameTimer = Random.Range(JetConfig.FlameIntervalMin.Value, JetConfig.FlameIntervalMax.Value);
                float scale = Random.Range(JetConfig.FlameScaleMin.Value, JetConfig.FlameScaleMax.Value);
                SpawnAtPoints(childLocator, blue ? JetpackThrusterEffect.FlameBlue : JetpackThrusterEffect.Flame, scale);
            }
        }

        private static void SpawnAtPoints(ChildLocator childLocator, GameObject prefab, float scale)
        {
            if (!prefab)
                return;
            foreach (var pointName in ThrusterPoints)
            {
                var point = childLocator.FindChild(pointName);
                if (!point)
                    continue;
                var instance = Instantiate(prefab, point);
                if (!Mathf.Approximately(scale, 1f))
                    instance.transform.localScale *= scale;
            }
        }

        private ChildLocator GetChildLocator()
        {
            if (!_childLocator && _modelLocator && _modelLocator.modelTransform)
                _childLocator = _modelLocator.modelTransform.GetComponent<ChildLocator>();
            return _childLocator;
        }
    }
}
