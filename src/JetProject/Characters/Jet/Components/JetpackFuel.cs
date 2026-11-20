using JetProject.Framework.UI;
using RoR2;
using UnityEngine;

namespace JetProject.Characters.Jet.Components
{
    /// <summary>
    /// Tanque de combustível da jetpack, medido em segundos de voo.
    /// Gasoline aumenta a capacidade (+% do tanque por stack); Hopoo Feather acelera a recarga (no lugar dos pulos extras). Só é simulado em quem tem autoridade
    /// sobre o body, pois o voo também é calculado lá.
    /// </summary>
    public sealed class JetpackFuel : MonoBehaviour, IResourceMeterSource
    {
        private static readonly Color FullColor = new Color(1f, 0.72f, 0.15f);
        private static readonly Color LowColor = new Color(1f, 0.25f, 0.1f);

        private CharacterBody _body;
        private float _timeSinceUse;
        private int _regenBlocks;
        private bool _initialized;

        public float Current { get; private set; }

        public float Capacity
        {
            get
            {
                // Gasoline: + % do tanque total (linear por stack).
                int gasoline = ItemCount(RoR2Content.Items.IgniteOnKill);
                return JetConfig.BaseTankSeconds.Value * (1f + JetConfig.TankPercentPerGasoline.Value * gasoline);
            }
        }

        /// <summary>Fração do tanque recarregada por segundo. Hopoo Feather: + % de velocidade (linear por stack).</summary>
        public float RegenTanksPerSecond
        {
            get
            {
                int feathers = ItemCount(RoR2Content.Items.Feather);
                return JetConfig.RegenTanksPerSecond.Value * (1f + JetConfig.RegenPercentPerFeather.Value * feathers);
            }
        }

        public float Fraction => Capacity > 0f ? Current / Capacity : 0f;

        public bool CanIgnite => Current >= JetConfig.MinFuelToIgnite.Value;

        public bool HasFuel => Current > 0f;

        Color IResourceMeterSource.MeterColor => Color.Lerp(LowColor, FullColor, Fraction);

        bool IResourceMeterSource.MeterVisible => _body && _body.healthComponent && _body.healthComponent.alive;

        private void Awake()
        {
            _body = GetComponent<CharacterBody>();
        }

        private void FixedUpdate()
        {
            if (!_body || !_body.hasEffectiveAuthority)
                return;

            float capacity = Capacity;
            if (!_initialized)
            {
                Current = capacity;
                _initialized = true;
            }

            if (_regenBlocks > 0)
                _timeSinceUse = 0f;   // bloqueado: também reinicia o atraso da recarga
            else
                _timeSinceUse += Time.fixedDeltaTime;

            if (_regenBlocks == 0 && _timeSinceUse >= JetConfig.RegenDelay.Value)
                Current += RegenTanksPerSecond * capacity * Time.fixedDeltaTime;

            Current = Mathf.Clamp(Current, 0f, capacity);
        }

        private int ItemCount(ItemDef item) =>
            _body && _body.inventory ? _body.inventory.GetItemCountEffective(item) : 0;

        /// <summary>
        /// Impede a recarga enquanto houver bloqueios ativos (contador: cada Block precisa de um Unblock).
        /// Ex.: mirar a secundária no ar.
        /// </summary>
        public void BlockRegen() => _regenBlocks++;

        public void UnblockRegen() => _regenBlocks = Mathf.Max(0, _regenBlocks - 1);

        /// <summary>Gasta combustível equivalente a <paramref name="seconds"/> de voo.</summary>
        public void Drain(float seconds)
        {
            Current = Mathf.Max(0f, Current - seconds);
            _timeSinceUse = 0f;
        }

        /// <summary>Esvazia o tanque e devolve a fração (0 a 1) que havia, para efeitos que escalam com o consumo.</summary>
        public float DrainAll()
        {
            float fraction = Fraction;
            Current = 0f;
            _timeSinceUse = 0f;
            return fraction;
        }
    }
}
