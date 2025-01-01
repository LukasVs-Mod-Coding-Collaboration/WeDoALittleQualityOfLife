/*
    WeDoALittleQualityOfLife is a Terraria Mod made with tModLoader.
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
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Events;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace WeDoALittleQualityOfLife.Common.ModSystems
{
    internal class WDALQOLNetworkingSystem
    {
        public void HandlePacket(BinaryReader reader, int whoAmI, Mod mod)
        {
            short type = reader.ReadInt16();
            float value = 0f;
            Vector2 RODCsoundPos = new Vector2(0f, 0f);
            Vector2 itemSpawnPos = new Vector2(0f, 0f);
            if(type == WDALQOLPacketTypeID.updateWindSpeedTarget)
            {
                value = reader.ReadSingle();
            }
            if(Main.netMode == NetmodeID.MultiplayerClient)
            {
                if (type == WDALQOLPacketTypeID.updateWindSpeedTarget)
                {
                    Main.windSpeedTarget = value;
                }
            }
            if(Main.netMode == NetmodeID.Server)
            {
                if(type == WDALQOLPacketTypeID.moondial)
                {
                    if (Main.moondialCooldown > 2)
                    {
                        Main.moondialCooldown = 2;
                    }
                }
                if(type == WDALQOLPacketTypeID.sundial)
                {
                    if(!Main.dontStarveWorld && Main.sundialCooldown > 2)
                    {
                        Main.sundialCooldown = 2;
                    }
                }
                if(type == WDALQOLPacketTypeID.weatherVane)
                {
                    if (Main.IsItRaining)
                    {
                        if (Main.maxRaining < 0.2f)
                        {
                            Main.maxRaining = 0.4f;
                        }
                        else if (Main.maxRaining < 0.6f)
                        {
                            Main.maxRaining = 0.8f;
                        }
                        else if (!Main.dontStarveWorld)
                        {
                            Main.StopRain();
                        }
                    }
                    else
                    {
                        Main.StartRain();
                        Main.maxRaining = 0.1f;
                    }
                }
                if(type == WDALQOLPacketTypeID.djinnLamp)
                {
                    float windSpeedPerMph = ((1.0f)/(50.0f));
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
                    }
                    ModPacket updateWindSpeedTargetPacket = mod.GetPacket();
                    updateWindSpeedTargetPacket.Write(WDALQOLPacketTypeID.updateWindSpeedTarget);
                    updateWindSpeedTargetPacket.Write((float)Main.windSpeedTarget);
                    updateWindSpeedTargetPacket.Send();
                }
                if(type == WDALQOLPacketTypeID.skyMill)
                {
                    float windSpeedPerMph = ((1.0f)/(50.0f));
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
                    }
                    else if (Math.Abs(Main.windSpeedTarget) >= windSpeedPerMph * 39.0f)
                    {
                        Main.windSpeedTarget = (-windSign) * windSpeedPerMph * 5.0f;
                    }
                    ModPacket updateWindSpeedTargetPacket = mod.GetPacket();
                    updateWindSpeedTargetPacket.Write(WDALQOLPacketTypeID.updateWindSpeedTarget);
                    updateWindSpeedTargetPacket.Write((float)Main.windSpeedTarget);
                    updateWindSpeedTargetPacket.Send();
                }
            }
        }
    }
}
