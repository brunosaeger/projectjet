using BepInEx.Configuration;
using JetProject.Framework.Core;

namespace JetProject.Characters.Jet
{
    /// <summary>
    /// Todos os números de balanceamento do Jet, em um só lugar.
    /// Expostos no config do BepInEx (BepInEx/config/com.jetproject.JetProject.cfg) para ajuste sem recompilar
    /// (basta reiniciar o jogo).
    ///
    /// Ao mudar QUALQUER valor padrão: incremente <see cref="DefaultsRevision"/> e registre em design/balance.md.
    ///
    /// É estático porque EntityStates e componentes são instanciados pelo jogo, sem injeção de dependência.
    /// </summary>
    internal static class JetConfig
    {
        private const int DefaultsRevision = 5;

        // Jetpack: voo
        public static ConfigEntry<float> FlightSpeedMultiplier;
        public static ConfigEntry<float> ClimbSpeed;
        public static ConfigEntry<float> FlightAcceleration;
        public static ConfigEntry<float> BoostSpeedMultiplier;
        public static ConfigEntry<float> BoostDuration;

        // Jetpack: combustível (em segundos de voo)
        public static ConfigEntry<float> BaseTankSeconds;
        public static ConfigEntry<float> TankPercentPerGasoline;
        public static ConfigEntry<float> RegenPercentPerFeather;
        public static ConfigEntry<float> MinFuelToIgnite;
        public static ConfigEntry<float> RegenDelay;
        public static ConfigEntry<float> RegenTanksPerSecond;

        // Primária
        public static ConfigEntry<float> PrimaryDamageCoefficient;
        public static ConfigEntry<float> PrimaryBaseDuration;
        public static ConfigEntry<float> PrimaryComboResetTime;
        public static ConfigEntry<float> PrimaryBulletRadius;
        public static ConfigEntry<float> PrimaryPiercingBulletRadius;
        public static ConfigEntry<float> PrimarySelfKnockback;
        public static ConfigEntry<float> PrimaryPiercingSelfKnockback;
        public static ConfigEntry<float> PrimaryPiercingDamageCoefficient;
        public static ConfigEntry<int> PrimaryPiercingHitCount;
        public static ConfigEntry<float> PrimaryCameraRecoil;
        public static ConfigEntry<float> PrimaryPiercingCameraRecoil;

        // Secundária: UES Safe Travels Supplies (sinalizador)
        public static ConfigEntry<float> SecondaryDamageCoefficient;
        public static ConfigEntry<int> SecondaryDirectHitCount;
        public static ConfigEntry<float> SecondaryAreaDamageCoefficient;
        public static ConfigEntry<bool> SecondaryAreaHitsDirectTarget;
        public static ConfigEntry<float> SecondaryCooldown;
        public static ConfigEntry<float> SecondaryBlastRadius;
        public static ConfigEntry<float> SecondaryProjectileSpeed;
        public static ConfigEntry<float> SecondaryProjectileScale;
        public static ConfigEntry<float> SecondarySelfKnockback;
        public static ConfigEntry<bool> SecondaryAirBackDodge;
        public static ConfigEntry<bool> SecondaryAirAim;
        public static ConfigEntry<float> SecondaryAimFallSpeed;
        public static ConfigEntry<float> SecondaryAimMaxDistance;
        public static ConfigEntry<float> SecondaryCameraRecoil;
        public static ConfigEntry<float> SecondaryFireZoneDamagePerSecond;
        public static ConfigEntry<float> SecondaryFireZoneProcCoefficient;
        public static ConfigEntry<float> SecondaryFireZoneDuration;
        public static ConfigEntry<float> SecondaryFireZoneScale;
        public static ConfigEntry<int> SecondaryFireSoundLayers;
        public static ConfigEntry<float> SecondaryChestChancePercent;
        public static ConfigEntry<float> SecondaryLegendaryChestChancePercent;

        public static void Bind(ConfigFile config)
        {
            const string flight = "Jet - Jetpack Flight";
            FlightSpeedMultiplier = config.Bind(flight, "FlightSpeedMultiplier", 2.2f, "Velocidade horizontal de voo (WASD) = velocidade de movimento × este valor.");
            ClimbSpeed = config.Bind(flight, "ClimbSpeed", 10f, "Velocidade de subida (m/s) enquanto a jetpack está ligada.");
            FlightAcceleration = config.Bind(flight, "FlightAcceleration", 60f, "Aceleração (m/s²) até a velocidade alvo do voo. Maior = mais responsivo, menor = mais inércia.");
            BoostSpeedMultiplier = config.Bind(flight, "BoostSpeedMultiplier", 1.8f, "Impulso inicial: só na PRIMEIRA ignição depois de sair do chão.");
            BoostDuration = config.Bind(flight, "BoostDuration", 0.35f, "Segundos até o impulso inicial voltar à velocidade normal de voo.");

            const string fuel = "Jet - Jetpack Fuel";
            BaseTankSeconds = config.Bind(fuel, "BaseTankSeconds", 3.75f, "Segundos de voo com o tanque cheio, sem itens.");
            TankPercentPerGasoline = config.Bind(fuel, "TankPercentPerGasoline", 0.15f, "Gasoline: +% da capacidade TOTAL do tanque por stack (0.15 = +15%, linear). Soma-se ao efeito normal do item.");
            RegenPercentPerFeather = config.Bind(fuel, "RegenPercentPerFeather", 0.2f, "Hopoo Feather: +% de velocidade de recarga por stack (0.2 = +20%, linear). Não dá pulos extras.");
            MinFuelToIgnite = config.Bind(fuel, "MinFuelToIgnite", 0.1f, "Combustível mínimo (segundos) para ligar a jetpack.");
            RegenDelay = config.Bind(fuel, "RegenDelay", 1f, "Segundos sem voar até começar a recarregar.");
            RegenTanksPerSecond = config.Bind(fuel, "RegenTanksPerSecond", 0.5f, "Fração do tanque recarregada por segundo (igual no chão e no ar).");

            const string primary = "Jet - Primary";
            PrimaryDamageCoefficient = config.Bind(primary, "DamageCoefficient", 3.5f, "Dano por tiro (3.5 = 350%).");
            PrimaryBaseDuration = config.Bind(primary, "BaseDuration", 0.5f, "Segundos entre tiros (antes de velocidade de ataque).");
            PrimaryComboResetTime = config.Bind(primary, "ComboResetTime", 1.5f, "Segundos sem atirar até a contagem do 3º tiro perfurante reiniciar.");
            PrimaryBulletRadius = config.Bind(primary, "BulletRadius", 0.4f, "Espessura (raio, em metros) da bala normal.");
            PrimaryPiercingBulletRadius = config.Bind(primary, "PiercingBulletRadius", 0.8f, "Espessura (raio, em metros) do 3º tiro perfurante.");
            PrimarySelfKnockback = config.Bind(primary, "SelfKnockback", 600f, "Recuo de cada tiro normal: força que empurra o Jet para trás (estilo Railgunner).");
            PrimaryPiercingSelfKnockback = config.Bind(primary, "PiercingSelfKnockback", 800f, "Recuo do 3º tiro (perfurante).");
            PrimaryPiercingDamageCoefficient = config.Bind(primary, "PiercingDamageCoefficient", 2.5f, "Dano de CADA acerto do 3º tiro (2.5 = 250%). Mantido abaixo de 400% para não ativar Kjaro's/Runald's Band.");
            PrimaryPiercingHitCount = config.Bind(primary, "PiercingHitCount", 2, "Quantos acertos o 3º tiro aplica (cada um com PiercingDamageCoefficient).");
            PrimaryCameraRecoil = config.Bind(primary, "CameraRecoil", 4f, "Coice da arma (câmera/mira sobe e volta). Referência: revólver da Bandit ≈ 4.");
            PrimaryPiercingCameraRecoil = config.Bind(primary, "PiercingCameraRecoil", 6.5f, "Coice da arma no 3º tiro.");

            const string secondary = "Jet - Secondary";
            SecondaryDamageCoefficient = config.Bind(secondary, "DamageCoefficient", 3f, "Dano de CADA acerto DIRETO do projétil (3 = 300%). Abaixo de 400% para não ativar Kjaro's/Runald's Band. Aplica Burn.");
            SecondaryDirectHitCount = config.Bind(secondary, "DirectHitCount", 2, "Acertos no inimigo atingido em cheio (2 = 2× 300%).");
            SecondaryAreaDamageCoefficient = config.Bind(secondary, "AreaDamageCoefficient", 3f, "Dano da explosão em área nos demais inimigos (3 = 300%, um acerto). Aplica Burn.");
            SecondaryAreaHitsDirectTarget = config.Bind(secondary, "AreaHitsDirectTarget", false, "Se true, o inimigo atingido em cheio também leva o dano em área (2×300% + 300%).");
            SecondaryCooldown = config.Bind(secondary, "Cooldown", 5f, "Recarga em segundos.");
            SecondaryBlastRadius = config.Bind(secondary, "BlastRadius", 7f, "Raio da explosão (m).");
            SecondaryProjectileSpeed = config.Bind(secondary, "ProjectileSpeed", 70f, "Velocidade de lançamento (m/s). O projétil cai com a gravidade (arco).");
            SecondaryProjectileScale = config.Bind(secondary, "ProjectileScale", 1.6f, "Tamanho do projétil (visual e colisão) em relação ao original.");
            SecondarySelfKnockback = config.Bind(secondary, "SelfKnockback", 2500f, "Recuo INTENSO no Jet (~25 m/s). Mirando para baixo, joga o Jet para cima.");
            SecondaryAirBackDodge = config.Bind(secondary, "AirBackDodge", true, "No AR, em vez do empurrão, faz o dash para trás do Commando (Tactical Dive, mesma animação e distância). No chão continua o empurrão (SelfKnockback).");
            SecondaryAirAim = config.Bind(secondary, "AirAim", true, "No AR: segurar a tecla mira (queda lenta, combustível sem recarga, mira do Captain + área); ao soltar, dispara e faz o dash.");
            SecondaryAimFallSpeed = config.Bind(secondary, "AimFallSpeed", 3f, "Velocidade máxima de queda (m/s) enquanto mira no ar. Queda normal chega a ~30+ m/s.");
            SecondaryAimMaxDistance = config.Bind(secondary, "AimMaxDistance", 150f, "Distância máxima da mira (m).");
            SecondaryCameraRecoil = config.Bind(secondary, "CameraRecoil", 3f, "Coice da câmera ao disparar.");
            SecondaryFireZoneDamagePerSecond = config.Bind(secondary, "FireZoneDamagePerSecond", 2f, "Poça de fogo no impacto (mini Molotov): dano por segundo (2 = 200%/s).");
            SecondaryFireZoneProcCoefficient = config.Bind(secondary, "FireZoneProcCoefficient", 0.5f, "Proc coefficient da poça; também é a chance de aplicar Burn por tick (0.5 = 50%).");
            SecondaryFireZoneDuration = config.Bind(secondary, "FireZoneDuration", 4f, "Duração da poça de fogo (s).");
            SecondaryFireZoneScale = config.Bind(secondary, "FireZoneScale", 0.5f, "Tamanho da poça em relação à do Molotov.");
            SecondaryFireSoundLayers = config.Bind(secondary, "FireSoundLayers", 2, "Volume do som de disparo (vanilla): quantas cópias tocam juntas. 1 = original, 2 ≈ +6 dB.");
            SecondaryChestChancePercent = config.Bind(secondary, "ChestChancePercent", 0.5f, "Chance (%) de surgir um baú não lendário quando o sinalizador mata. Rolada uma vez por disparo.");
            SecondaryLegendaryChestChancePercent = config.Bind(secondary, "LegendaryChestChancePercent", 0.25f, "Chance (%) de surgir um baú LENDÁRIO, no mesmo sorteio (independente da chance acima).");

            ConfigRevision.ResetIfOutdated(config, DefaultsRevision);
        }
    }
}
