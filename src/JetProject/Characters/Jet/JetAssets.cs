using System.Collections.Generic;
using System.Linq;
using JetProject.Framework.Assets;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using UnityEngine;

namespace JetProject.Characters.Jet
{
    /// <summary>
    /// Referências a assets do jogo usados pelo Jet, carregadas uma vez no registro.
    /// Tracers, efeitos e o projétil-base são PROVISÓRIOS (emprestados do jogo) até existirem assets próprios.
    /// Sons do Jet não ficam aqui: são eventos Wwise (ver <see cref="JetSounds"/>).
    /// </summary>
    internal static class JetAssets
    {
        public static GameObject HandCannonTracer { get; private set; }
        public static GameObject HandCannonPiercingTracer { get; private set; }

        /// <summary>Projétil-base clonado para o sinalizador (provisório: firebolt da Artificer).</summary>
        public static string SupplyFlareBaseProjectileKey => RoR2_Base_Mage.MageFireboltBasic_prefab;
        public static GameObject SupplyFlareExplosionEffect { get; private set; }

        /// <summary>Base da poça de fogo da secundária: zona de dano do Molotov.</summary>
        public static string SupplyFireZoneBaseKey => RoR2_DLC1_Molotov.MolotovProjectileDotZone_prefab;

        /// <summary>Mira da secundária no ar: crosshair do Orbital Probe do Captain.</summary>
        public static GameObject SupplyFlareAimCrosshair { get; private set; }

        /// <summary>Círculo no ponto de impacto (escalado pelo raio da explosão): indicador do morteiro da REX.</summary>
        public static GameObject SupplyFlareAreaIndicator { get; private set; }

        /// <summary>Baús possíveis do sinalizador (exceto lendário). Os que exigem DLC são filtrados na hora do spawn.</summary>
        public static IReadOnlyList<SpawnCard> SupplyChests { get; private set; }
        public static SpawnCard SupplyLegendaryChest { get; private set; }

        public static void Load()
        {
            HandCannonTracer = GameAssets.Load<GameObject>(RoR2_Base_Bandit2.TracerBanditPistol_prefab);
            HandCannonPiercingTracer = GameAssets.Load<GameObject>(RoR2_DLC1_Railgunner.TracerRailgunLight_prefab);

            SupplyFlareExplosionEffect = GameAssets.Load<GameObject>(RoR2_Base_IgniteOnKill.IgniteExplosionVFX_prefab);
            SupplyFlareAimCrosshair = GameAssets.Load<GameObject>(RoR2_Base_Captain.CaptainAirstrikeCrosshair_prefab);
            SupplyFlareAreaIndicator = GameAssets.Load<GameObject>(RoR2_Base_Treebot.TreebotMortarAreaIndicator_prefab);

            SupplyChests = new[]
            {
                RoR2_Base_Chest1.iscChest1_asset,
                RoR2_Base_Chest2.iscChest2_asset,
                RoR2_Base_Chest1StealthedVariant.iscChest1Stealthed_asset,
                RoR2_Base_CategoryChest.iscCategoryChestDamage_asset,
                RoR2_Base_CategoryChest.iscCategoryChestHealing_asset,
                RoR2_Base_CategoryChest.iscCategoryChestUtility_asset,
                RoR2_Base_EquipmentBarrel.iscEquipmentBarrel_asset,
                RoR2_Base_CasinoChest.iscCasinoChest_asset,
                RoR2_Base_LunarChest.iscLunarChest_asset,
                RoR2_DLC1_VoidChest.iscVoidChest_asset,
            }.Select(GameAssets.Load<SpawnCard>).Where(card => card).ToArray();

            SupplyLegendaryChest = GameAssets.Load<SpawnCard>(RoR2_Base_GoldChest.iscGoldChest_asset);
        }
    }
}
