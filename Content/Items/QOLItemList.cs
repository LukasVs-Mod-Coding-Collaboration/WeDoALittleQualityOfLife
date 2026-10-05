/*
    WeDoALittleTrolling is a Terraria Mod made with tModLoader.
    Copyright (C) 2022-2025 LukasV-Coding

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using WeDoALittleQualityOfLife.Content.Tiles;
using WeDoALittleQualityOfLife.Common.Configs;

namespace WeDoALittleQualityOfLife.Content.Items
{
    internal class QOLItemList : GlobalItem
    {
        public override bool InstancePerEntity => false;

        public override bool ConsumeItem(Item item, Player player)
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableUtilityTiles)
            {
                return base.ConsumeItem(item, player);
            }
            if (BuffTiles.BuffTilesItemIDs.Contains(item.type))
            {
                SoundStyle buffActivateSoundStyle = SoundID.Item4;
                bool playSound = true;
                switch (item.type)
                {
                    case ItemID.WarTable:
                        buffActivateSoundStyle = SoundID.Item4;
                        break;
                    case ItemID.BewitchingTable:
                        buffActivateSoundStyle = SoundID.Item4;
                        break;
                    case ItemID.SharpeningStation:
                        buffActivateSoundStyle = SoundID.Item37;
                        break;
                    case ItemID.CrystalBall:
                        buffActivateSoundStyle = SoundID.Item4;
                        break;
                    case ItemID.AmmoBox:
                        buffActivateSoundStyle = SoundID.Item149;
                        break;
                    case ItemID.SliceOfCake:
                        buffActivateSoundStyle = SoundID.Item2;
                        break;
                    default:
                        playSound = false;
                        break;
                }
                if (playSound)
                {
                    SoundEngine.PlaySound(buffActivateSoundStyle, player.Center);
                }
                return false;
            }
            return base.ConsumeItem(item, player);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableUtilityTiles)
            {
                return;
            }
            if (item.type == ItemID.Moondial)
            {
                List<TooltipLine> infoLine = tooltips.FindAll(t => (t.Name == "Tooltip0") && (t.Mod == "Terraria"));
                infoLine.ForEach(t => t.Text = "Allows time to fast forward to dusk");
            }
            if (item.type == ItemID.Sundial)
            {
                List<TooltipLine> infoLine = tooltips.FindAll(t => (t.Name == "Tooltip0") && (t.Mod == "Terraria"));
                infoLine.ForEach(t => t.Text = "Allows time to fast forward to dawn");
            }
            if (item.type == ItemID.WeatherVane)
            {
                TooltipLine tileFunctionLine = new TooltipLine(Mod, "TileFunctionDescription", "Allows rain to start, intesify and stop");
                tooltips.Add(tileFunctionLine);
            }
            if (item.type == ItemID.SkyMill)
            {
                TooltipLine tileFunctionLine = new TooltipLine(Mod, "TileFunctionDescription", "Allows wind to start, intesify, change direction and stop");
                tooltips.Add(tileFunctionLine);
            }
            if (item.type == ItemID.DjinnLamp)
            {
                TooltipLine tileFunctionLine = new TooltipLine(Mod, "TileFunctionDescription", "Allows sandstorms to start and stop");
                tooltips.Add(tileFunctionLine);
            }
        }

        public override void SetDefaults(Item item)
        {
            if (!ModContent.GetInstance<WDALQOLServerConfig>().DisableInfiniteBossSummoningItems)
            {
                // Make boss summoning items non-consumable
                if
                (
                    item.type == ItemID.SlimeCrown ||
                    item.type == ItemID.SuspiciousLookingEye ||
                    item.type == ItemID.WormFood ||
                    item.type == ItemID.BloodySpine ||
                    item.type == ItemID.Abeemination ||
                    item.type == ItemID.DeerThing ||
                    item.type == ItemID.MechanicalWorm ||
                    item.type == ItemID.MechanicalEye ||
                    item.type == ItemID.MechanicalSkull ||
                    item.type == ItemID.MechdusaSummon ||
                    item.type == ItemID.CelestialSigil ||
                    item.type == ItemID.PumpkinMoonMedallion ||
                    item.type == ItemID.NaughtyPresent ||
                    item.type == ItemID.GoblinBattleStandard ||
                    item.type == ItemID.PirateMap ||
                    item.type == ItemID.BloodMoonStarter
                )
                {
                    item.consumable = false;
                    item.maxStack = 1;
                }
            }
            if (!ModContent.GetInstance<WDALQOLServerConfig>().DisableUtilityTiles)
            {
                //Make Buff Furniture give their Buffs when used
                if (BuffTiles.BuffTilesItemIDs.Contains(item.type))
                {
                    item.buffTime = 108000;
                    switch (item.type)
                    {
                        case ItemID.WarTable:
                            item.buffType = BuffID.WarTable;
                            break;
                        case ItemID.BewitchingTable:
                            item.buffType = BuffID.Bewitched;
                            break;
                        case ItemID.SharpeningStation:
                            item.buffType = BuffID.Sharpened;
                            break;
                        case ItemID.CrystalBall:
                            item.buffType = BuffID.Clairvoyance;
                            break;
                        case ItemID.AmmoBox:
                            item.buffType = BuffID.AmmoBox;
                            break;
                        case ItemID.SliceOfCake:
                            item.buffType = BuffID.SugarRush;
                            item.buffTime = 7200;
                            break;
                        default:
                            item.buffTime = 0;
                            break;
                    }
                }
            }
        }

        public override bool ReforgePrice(Item item, ref long reforgePrice, ref bool canApplyDiscount)
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableCustomReforgePrices)
            {
                return base.ReforgePrice(item, ref reforgePrice, ref canApplyDiscount);
            }
            canApplyDiscount = false;
            reforgePrice = (long)Item.buyPrice(silver: 15);
            if (NPC.downedBoss1)
            {
                reforgePrice = (long)Item.buyPrice(silver: 20);
            }
            if (NPC.downedBoss2)
            {
                reforgePrice = (long)Item.buyPrice(silver: 25);
            }
            if (NPC.downedBoss3 || Main.hardMode)
            {
                reforgePrice *= 2L;
            }
            if (Main.hardMode)
            {
                reforgePrice *= 2L;
            }
            if (NPC.downedPlantBoss)
            {
                reforgePrice *= 2L;
            }
            if (NPC.downedMoonlord)
            {
                reforgePrice *= 2L;
            }
            return false;
        }
    }
}
