/*
    WeDoALittleQualityOfLife is a Terraria Mod made with tModLoader.
    Copyright (C) 2022-2024 LukasV-Coding

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

using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using WeDoALittleQualityOfLife;
using Terraria.ID;

namespace WeDoALittleQualityOfLife.Common.ModSystems
{
    internal static class WDALQOLIntermediateLanguageEditing
    {
        public static void RegisterILHooks()
        {
            IL_WorldGen.UpdateWorld_Inner += IL_WorldGen_UpdateWorld;
            IL_Player.UpdateBiomes += IL_Player_UpdateBiomes;
            IL_Main.UpdateTime_SpawnTownNPCs += IL_Main_UpdateTime_SpawnTownNPCs;
        }

        public static void UnregisterILHooks()
        {
            IL_WorldGen.UpdateWorld_Inner -= IL_WorldGen_UpdateWorld;
            IL_Player.UpdateBiomes -= IL_Player_UpdateBiomes;
            IL_Main.UpdateTime_SpawnTownNPCs -= IL_Main_UpdateTime_SpawnTownNPCs;
        }

        public static void IL_WorldGen_UpdateWorld(ILContext intermediateLanguageContext)
        {
            bool successInjectInfectionSpreadHook = true;
            try
            {
                ILCursor cursor = new ILCursor(intermediateLanguageContext);
                cursor.GotoNext(i => i.MatchStsfld<WorldGen>(nameof(WorldGen.AllowedToSpreadInfections))); //Go to the place right after the "AllowedToSpreadInfections" variable is set.
                cursor.Index++; //Go in front of it now.
                cursor.Emit(OpCodes.Ldc_I4_0); //set "false" as the parameter to write.
                cursor.Emit(OpCodes.Stsfld, typeof(WorldGen).GetField(nameof(WorldGen.AllowedToSpreadInfections))); //Write "false" into the "AllowedToSpreadInfections" variable.
                cursor.GotoNext(i => i.MatchStsfld<WorldGen>(nameof(WorldGen.AllowedToSpreadInfections))); //Do the same if StopBiomeSpreadPower is enabled.
                cursor.Index++;
                cursor.Emit(OpCodes.Ldc_I4_0);
                cursor.Emit(OpCodes.Stsfld, typeof(WorldGen).GetField(nameof(WorldGen.AllowedToSpreadInfections)));
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<WeDoALittleQualityOfLife>(), intermediateLanguageContext);
                WeDoALittleQualityOfLife.logger.Fatal("WDALT: Failed to inject Infection Spread Hook. Broken IL Code has been dumped to tModLoader-Logs/ILDumps/WeDoALittleQualityOfLife.");
                successInjectInfectionSpreadHook = false;
            }
            if(successInjectInfectionSpreadHook)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Successfully injected Infection Spread Hook via IL Editing.");
            }
        }

        public static void IL_Player_UpdateBiomes(ILContext intermediateLanguageContext)
        {
            bool successInjectGetGoodWorldLightingHook = true;
            try
            {
                ILCursor cursor = new ILCursor(intermediateLanguageContext);
                cursor.GotoNext(i => i.MatchLdsfld<Main>(nameof(Main.getGoodWorld)));
                cursor.Index++; //move cursor to the "Main.getGoodWorld" if statement.
                cursor.Emit(OpCodes.Pop); //Pop the value of Main.getGoodWorld off the stack.
                cursor.Emit(OpCodes.Ldc_I4_0); //Push "false" onto the stack. This causes the if statement to never run the code inside.
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<WeDoALittleQualityOfLife>(), intermediateLanguageContext);
                WeDoALittleQualityOfLife.logger.Fatal("WDALT: Failed to inject For The Worthy Lighting Hook. Broken IL Code has been dumped to tModLoader-Logs/ILDumps/WeDoALittleQualityOfLife.");
                successInjectGetGoodWorldLightingHook = false;
            }
            if(successInjectGetGoodWorldLightingHook)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Successfully injected For The Worthy Lighting Hook via IL Editing.");
            }
        }

        public static void IL_Main_UpdateTime_SpawnTownNPCs(ILContext intermediateLanguageContext)
        {
            bool successInjectTownNPCsRespawnTimeHook = true;
            try
            {
                ILCursor cursor = new ILCursor(intermediateLanguageContext);
                cursor.GotoNext(i => i.MatchLdcR8(7200.0)); //move cursor towards the town NPC spawn time intervall (7200 / 60 = 120 seconds)
                cursor.Index++; //move cursor after the town NPC spawn time intervall.
                cursor.Emit(OpCodes.Pop); //Pop 7200 off the stack.
                cursor.Emit(OpCodes.Ldc_R8, 900.0); //Push 900 onto the stack. This causes the town NPC spawn time intervall to reduce to 900 / 60 = 15 seconds.
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<WeDoALittleQualityOfLife>(), intermediateLanguageContext);
                WeDoALittleQualityOfLife.logger.Fatal("WDALT: Failed to inject Town NPCs Respawn Time Hook. Broken IL Code has been dumped to tModLoader-Logs/ILDumps/WeDoALittleQualityOfLife.");
                successInjectTownNPCsRespawnTimeHook = false;
            }
            if(successInjectTownNPCsRespawnTimeHook)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Successfully injected Town NPCs Respawn Time Hook via IL Editing.");
            }
        }
    }
}
