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

using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WeDoALittleQualityOfLife.Common.Configs;
using System.Collections.Generic;

namespace WeDoALittleQualityOfLife.Common.Utilities
{
    internal class WDALQOLPlayerUtil : ModPlayer
    {
        public Player player;
        public int timeSinceRespawn = 0;

        public static readonly int[] availableBuffTileBuffs =
        {
            BuffID.WarTable,
            BuffID.Bewitched,
            BuffID.Sharpened,
            BuffID.Clairvoyance,
            BuffID.AmmoBox,
            BuffID.DeadCellsPotionStation,
            BuffID.SugarRush
        };

        public List<int> buffTileBuffs = new List<int>();
        public int remainingSugarRushTime = 0;
        
        public override void Initialize()
        {
            player = this.Player;
            timeSinceRespawn = 0;
            //Terraria 1.4.5.8 has a bug where chatOverhead.color is NULL when a player object is initialized, explicitly initialize it so we can use chatOverhead right away.
            player.chatOverhead.color = Color.White;
        }

        public static void RegisterHooks()
        {
            On_Player.AllowsRespawnTimerSkip += On_Player_AllowsRespawnTimerSkip;
            On_Player.KillMe += On_Player_KillMe;
            On_Player.Spawn += On_Player_Spawn;
        }

        public static void UnregisterHooks()
        {
            On_Player.AllowsRespawnTimerSkip -= On_Player_AllowsRespawnTimerSkip;
            On_Player.KillMe -= On_Player_KillMe;
            On_Player.Spawn -= On_Player_Spawn;
        }

        public static void On_Player_KillMe(Terraria.On_Player.orig_KillMe orig, Terraria.Player self, Terraria.DataStructures.PlayerDeathReason damageSource, double dmg, int hitDirection, bool pvp)
        {
            if (!ModContent.GetInstance<WDALQOLServerConfig>().DisableUtilityTiles && self.TryGetModPlayer<WDALQOLPlayerUtil>(out WDALQOLPlayerUtil util))
            {
                util.CollectBuffTileBuffs();
            }
            orig.Invoke(self, damageSource, dmg, hitDirection, pvp);
        }

        public static void On_Player_Spawn(Terraria.On_Player.orig_Spawn orig, Terraria.Player self, Terraria.PlayerSpawnContext context)
        {
            if (!ModContent.GetInstance<WDALQOLServerConfig>().DisableUtilityTiles && self.TryGetModPlayer<WDALQOLPlayerUtil>(out WDALQOLPlayerUtil util))
            {
                util.RestoreBuffTileBuffs();
            }
            orig.Invoke(self, context);
        }

        internal static bool Terraria_AnyBossHindersRespawnTime(Player player)
        {
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if
                (
                    npc != null &&
                    npc.active &&
                    npc.type != NPCID.MartianSaucerCore &&
                    (
                        npc.boss ||
                        npc.type == NPCID.EaterofWorldsHead ||
                        npc.type == NPCID.EaterofWorldsBody ||
                        npc.type == NPCID.EaterofWorldsTail
                    ) &&
                    (
                        Math.Abs(player.Center.X - npc.Center.X) + Math.Abs(player.Center.Y - npc.Center.Y) < 4000f ||
                        npc.interactedWithPlayerLocally
                    )
                )
                {
                    return true;
                }
            }
            return false;
        }

        public static bool On_Player_AllowsRespawnTimerSkip(Terraria.On_Player.orig_AllowsRespawnTimerSkip orig, Terraria.Player self)
        {
            bool ret = orig.Invoke(self);
            if (!ModContent.GetInstance<WDALQOLServerConfig>().DisableRespawnSpeedupNoBossActive && self.deadTime >= Terraria.Player.DeadSkipLockoutTime)
            {
                ret = !Terraria_AnyBossHindersRespawnTime(self);
            }
            return ret;
        }

        public void ClearBuffTileBuffs()
        {
            buffTileBuffs.Clear();
            remainingSugarRushTime = 0;
        }

        public void CollectBuffTileBuffs()
        {
            ClearBuffTileBuffs();
            for (int i = 0; i < availableBuffTileBuffs.Length; i++)
            {
                if (player.HasBuff(availableBuffTileBuffs[i]))
                {
                    buffTileBuffs.Add(availableBuffTileBuffs[i]);
                    if (availableBuffTileBuffs[i] == BuffID.SugarRush)
                    {
                        int idx = player.FindBuffIndex(availableBuffTileBuffs[i]);
                        if (idx >= 0 && idx < player.buffTime.Length)
                        {
                            remainingSugarRushTime = player.buffTime[idx];
                        }
                    }
                }
            }
        }

        public void RestoreBuffTileBuffs()
        {
            foreach (int buffID in buffTileBuffs)
            {
                if(!player.HasBuff(buffID))
                {
                    if (buffID == BuffID.SugarRush)
                    {
                        player.AddBuff(buffID, remainingSugarRushTime);
                    }
                    else
                    {
                        player.AddBuff(buffID, 108000);
                    }
                }
            }
            ClearBuffTileBuffs();
        }

        public static bool IsBossActive()
        {
            for(int i = 0;i < Main.npc.Length; i++)
            {
                if
                (
                    Main.npc[i].active &&
                    (
                        Main.npc[i].boss ||
                        Main.npc[i].type == NPCID.EaterofWorldsHead ||
                        Main.npc[i].type == NPCID.EaterofWorldsBody ||
                        Main.npc[i].type == NPCID.EaterofWorldsTail
                    )
                )
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnRespawn()
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableRespawnWithFullHealth)
            {
                base.OnRespawn();
                return;
            }
            timeSinceRespawn = 1;
            base.OnRespawn();
        }

        public override void UpdateLifeRegen()
        {
            if (timeSinceRespawn > 10)
            {
                timeSinceRespawn = 0;
                player.Heal(999999);
            }
            else if (timeSinceRespawn > 0)
            {
                timeSinceRespawn++;
            }
            base.UpdateLifeRegen();
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableRespawnSpeedup)
            {
                base.Kill(damage, hitDirection, pvp, damageSource);
                return;
            }
            bool canSkipRespawn = player.AllowsRespawnTimerSkip();
            if(canSkipRespawn)
            {
                player.respawnTimer = 180;
            }
            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void UpdateDead()
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableRespawnSpeedup)
            {
                base.UpdateDead();
                return;
            }
            bool canSkipRespawn = player.AllowsRespawnTimerSkip();
            if (canSkipRespawn && player.respawnTimer > 180)
            {
                player.respawnTimer = 180;
            }
            base.UpdateDead();
        }

        public override void UpdateEquips()
        {
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableIceBiomeWaterNoSlowness)
            {
                base.UpdateEquips();
                return;
            }
            player.arcticDivingGear = true;
            base.UpdateEquips();
        }
    }
}
