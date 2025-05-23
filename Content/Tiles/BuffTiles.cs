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
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using WeDoALittleQualityOfLife.Common.ModSystems;

namespace WeDoALittleQualityOfLife.Content.Tiles
{
    internal class BuffTiles : GlobalTile
    {
        public static readonly int[] BuffTilesItemIDs =
        {
            ItemID.BewitchingTable,
            ItemID.WarTable,
            ItemID.SharpeningStation,
            ItemID.CrystalBall,
            ItemID.AmmoBox,
            ItemID.SliceOfCake
        };

        public override void PlaceInWorld(int i, int j, int type, Item item)
        {
            if (BuffTilesItemIDs.Contains(item.type))
            {
                ReduceStack(item);
            }
            base.PlaceInWorld(i, j, type, item);
        }

        public static void ReduceStack(Item item)
        {
            if (item.stack > 1)
            {
                item.stack--;
            }
            else
            {
                item.type = ItemID.None;
                item.stack = 0;
                item.active = false;
            }
        }

        public override void RightClick(int i, int j, int type)
        {
            if (type == TileID.Moondial)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (Main.moondialCooldown > 2)
                    {
                        Main.moondialCooldown = 2;
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket moondialPacket = Mod.GetPacket();
                    moondialPacket.Write(WDALQOLPacketTypeID.moondial);
                    moondialPacket.Send();
                }

            }
            else if (type == TileID.Sundial)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (!Main.dontStarveWorld && Main.sundialCooldown > 2)
                    {
                        Main.sundialCooldown = 2;
                    }
                }
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket sundialPacket = Mod.GetPacket();
                    sundialPacket.Write(WDALQOLPacketTypeID.sundial);
                    sundialPacket.Send();
                }
            }
            else if (type == TileID.WeatherVane)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    if (Main.IsItRaining)
                    {
                        if (Main.maxRaining < 0.2f)
                        {
                            Main.maxRaining = 0.4f;
                            SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                        }
                        else if (Main.maxRaining < 0.6f)
                        {
                            Main.maxRaining = 0.8f;
                            SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                        }
                        else if (!Main.dontStarveWorld)
                        {
                            Main.StopRain();
                            SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                        }
                    }
                    else
                    {
                        Main.StartRain();
                        Main.maxRaining = 0.1f;
                        SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                    }
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket weatherVanePacket = Mod.GetPacket();
                    weatherVanePacket.Write(WDALQOLPacketTypeID.weatherVane);
                    weatherVanePacket.Send();
                    SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                }
            }
            else if (type == TileID.DjinnLamp)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    float windSpeedPerMph = ((1.0f) / (50.0f));
                    float windSign = 1.0f;
                    if (Main.windSpeedTarget >= 0)
                    {
                        windSign = 1;
                    }
                    else
                    {
                        windSign = -1;
                    }
                    if (Sandstorm.Happening)
                    {
                        Sandstorm.StopSandstorm();
                        SoundEngine.PlaySound(SoundID.Item20, new Vector2(i * 16, j * 16));
                    }
                    else
                    {
                        Sandstorm.StartSandstorm();
                        if (Math.Abs(Main.windSpeedTarget) < windSpeedPerMph * 30.0f)
                        {
                            if (Main.windSpeedTarget > 0)
                            {
                                Main.windSpeedTarget = windSign * windSpeedPerMph * 35.0f;

                            }
                            else
                            {
                                Main.windSpeedTarget = windSign * windSpeedPerMph * 35.0f;
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item20, new Vector2(i * 16, j * 16));
                    }
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket djinnLampPacket = Mod.GetPacket();
                    djinnLampPacket.Write(WDALQOLPacketTypeID.djinnLamp);
                    djinnLampPacket.Send();
                    SoundEngine.PlaySound(SoundID.Item20, new Vector2(i * 16, j * 16));
                }
            }
            else if (type == TileID.SkyMill)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    float windSpeedPerMph = ((1.0f) / (50.0f));
                    float windSign = 1.0f;
                    if (Main.windSpeedTarget >= 0)
                    {
                        windSign = 1;
                    }
                    else
                    {
                        windSign = -1;
                    }
                    if (Math.Abs(Main.windSpeedTarget) < windSpeedPerMph * 39.0f)
                    {
                        Main.windSpeedTarget += windSign * windSpeedPerMph * 10.0f;
                        SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                    }
                    else if (Math.Abs(Main.windSpeedTarget) >= windSpeedPerMph * 39.0f)
                    {
                        Main.windSpeedTarget = (-windSign) * windSpeedPerMph * 5.0f;
                        SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                    }
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket skyMillPacket = Mod.GetPacket();
                    skyMillPacket.Write(WDALQOLPacketTypeID.skyMill);
                    skyMillPacket.Send();
                    SoundEngine.PlaySound(SoundID.Item4, new Vector2(i * 16, j * 16));
                }
            }
            base.RightClick(i, j, type);
        }
    }
}

