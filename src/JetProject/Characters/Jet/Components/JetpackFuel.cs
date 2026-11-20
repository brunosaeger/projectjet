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
        private static readonly Color DisabledColor = new Color(0.45f, 0.45f, 0.45f);

        private CharacterBody _body;
        private float _timeSinceUse;
        private int _regenBlocks;
        private float _disabledUntil;
        private bool _initialized;

        /// <summary>Jetpack desativada (ex.: após a queima do Ace the Skies): não liga e não recarrega.</summary>
        public bool IsDisabled => Time.fixedTime < _disabledUntil;

        public float Current { get; private set; }

        /// <summary>
        /// Disparado quando o gasto de combustível cruza para baixo de <c>LowFuelWarningFraction</c> do tanque.
        /// Só na autoridade (onde o combustível é simulado). Dispara de novo depois de recarregar acima do limite.
        /// </summary>
        public event System.Action LowFuelReached;

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

        public bool CanIgnite => !IsDisabled && Current >= JetConfig.MinFuelToIgnite.Value;

        public bool HasFuel => Current > 0f;

        Color IResourceMeterSource.MeterColor => IsDisabled ? DisabledColor : Color.Lerp(LowColor, FullColor, Fraction);

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

            bool blocked = _regenBlocks > 0 || IsDisabled;
            if (blocked)
                _timeSinceUse = 0f;   // bloqueado: também reinicia o atraso da recarga
            else
                _timeSinceUse += Time.fixedDeltaTime;

            if (!blocked && _timeSinceUse >= JetConfig.RegenDelay.Value)
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

        /// <summary>Desativa a jetpack por <paramref name="seconds"/> (não liga, não recarrega). Só na autoridade.</summary>
        public void DisableFor(float seconds) => _disabledUntil = Mathf.Max(_disabledUntil, Time.fixedTime + seconds);

        /// <summary>Gasta combustível equivalente a <paramref name="seconds"/> de voo.</summary>
        public void Drain(float seconds)
        {
            float before = Fraction;
            Current = Mathf.Max(0f, Current - seconds);
            _timeSinceUse = 0f;

            float threshold = JetConfig.LowFuelWarningFraction.Value;
            if (before >= threshold && Fraction < threshold)
                LowFuelReached?.Invoke();
        }
    }
}
