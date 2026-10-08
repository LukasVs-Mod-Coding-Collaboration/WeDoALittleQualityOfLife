using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace WeDoALittleQualityOfLife.Common.Configs
{
    public class WDALQOLServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;


        [Header("World")]

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableEvilBiomeSpreadPrevention;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableDyePlantGrowthSpeedup;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableNPCArrivalSpeedup;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableForTheWorthyDarknessDefuser;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableForTheWorthyBossGriefingDefuser;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableBoulderAndMeteorRainDefuser;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableUtilityTiles;

        [Header("Player")]

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableRespawnSpeedup;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableRespawnWithFullHealth;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableIceBiomeWaterNoSlowness;

        [Header("Recipes")]

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableExtraRecipes;

        [Header("Economy")]

        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableCustomReforgePrices;
        [DefaultValue(false)]
        [ReloadRequired]
        public bool DisableInfiniteBossSummoningItems;
        
    }
}
