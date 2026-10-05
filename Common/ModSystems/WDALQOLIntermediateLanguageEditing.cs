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

using Terraria;
using Terraria.ModLoader;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using WeDoALittleQualityOfLife.Common.Configs;

namespace WeDoALittleQualityOfLife.Common.ModSystems
{
    internal static class WDALQOLIntermediateLanguageEditing
    {
        public static void RegisterILHooks()
        {
            IL_WorldGen.UpdateWorld_Inner += IL_WorldGen_UpdateWorld;
            IL_SceneState.UpdateLightDecay += IL_SceneState_UpdateLightDecay;
            IL_Main.UpdateTime_SpawnTownNPCs += IL_Main_UpdateTime_SpawnTownNPCs;
            IL_WorldGen.UpdateWorld_OvergroundTile += IL_WorldGen_UpdateWorld_OvergroundTile;
            IL_WorldGen.UpdateWorld_UndergroundTile += IL_WorldGen_UpdateWorld_UndergroundTile;
        }

        public static void UnregisterILHooks()
        {
            IL_WorldGen.UpdateWorld_Inner -= IL_WorldGen_UpdateWorld;
            IL_SceneState.UpdateLightDecay -= IL_SceneState_UpdateLightDecay;
            IL_Main.UpdateTime_SpawnTownNPCs -= IL_Main_UpdateTime_SpawnTownNPCs;
            IL_WorldGen.UpdateWorld_OvergroundTile -= IL_WorldGen_UpdateWorld_OvergroundTile;
            IL_WorldGen.UpdateWorld_UndergroundTile -= IL_WorldGen_UpdateWorld_UndergroundTile;
        }

        public static void IL_WorldGen_UpdateWorld(ILContext intermediateLanguageContext) /* [SKIPPED] TAPI_1.4.5.8: NEEDS VERIFICATION */
        {
            bool successInjectInfectionSpreadHook = true;
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableEvilBiomeSpreadPrevention)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Infection Spread Hook is disabled in the server configuration, skipping injection...");
                return;
            }
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

        public static void IL_SceneState_UpdateLightDecay(ILContext intermediateLanguageContext) /* [OK] TAPI_1.4.5.8: VERIFIED */
        {
            bool successInjectGetGoodWorldLightingHook = true;
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableForTheWorthyDarknessDefuser)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: For The Worthy Lighting Hook is disabled in the server configuration, skipping injection...");
                return;
            }
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

        public static void IL_Main_UpdateTime_SpawnTownNPCs(ILContext intermediateLanguageContext) /* [SKIPPED] TAPI_1.4.5.8: NEEDS VERIFICATION */
        {
            bool successInjectTownNPCsRespawnTimeHook = true;
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableNPCArrivalSpeedup)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Town NPCs Respawn Time Hook is disabled in the server configuration, skipping injection...");
                return;
            }
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

        public static void IL_WorldGen_UpdateWorld_OvergroundTile(ILContext intermediateLanguageContext) /* [SKIPPED] TAPI_1.4.5.8: NEEDS VERIFICATION */
        {
            bool successInjectPlantOvergroundHook = true;
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableDyePlantGrowthSpeedup)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Strange Plant Overground Hook is disabled in the server configuration, skipping injection...");
                return;
            }
            try
            {
                ILCursor cursor = new ILCursor(intermediateLanguageContext);
                cursor.GotoNext(i => i.MatchLdcI4(3000));
                cursor.Index++;
                cursor.GotoNext(i => i.MatchLdcI4(3000)); //Move to the position where the number 3000 is pushed onto the stack for RNG.
                cursor.Index++; //Move after it now.
                cursor.Emit(OpCodes.Pop); //Pop Terrarias RNG chance denominator 15000 off the stack.
                int rngDenominator1 = 2; //Set 2 as the denominator for RNG. 1 in 2 chance = 50%
                cursor.Emit(OpCodes.Ldc_I4, rngDenominator1); //Finally, push our denominator onto the stack instead.
                cursor.GotoNext(i => i.MatchLdcI4(15000)); //Move to the position where the number 15000 is pushed onto the stack for RNG.
                cursor.Index++; //Move after it now.
                cursor.Emit(OpCodes.Pop); //Pop Terrarias RNG chance denominator 15000 off the stack.
                int rngDenominator2 = 1; //Set 1 as the denominator for RNG. 1 in 1 chance = 100%
                cursor.Emit(OpCodes.Ldc_I4, rngDenominator2); //Finally, push our denominator onto the stack instead.
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<WeDoALittleQualityOfLife>(), intermediateLanguageContext);
                WeDoALittleQualityOfLife.logger.Fatal("WDALT: Failed to inject Strange Plant Overground Hook. Broken IL Code has been dumped to tModLoader-Logs/ILDumps/WeDoALittleQualityOfLife.");
                successInjectPlantOvergroundHook = false;
            }
            if(successInjectPlantOvergroundHook)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Successfully injected Strange Plant Overground Hook via IL Editing.");
            }
        }

        public static void IL_WorldGen_UpdateWorld_UndergroundTile(ILContext intermediateLanguageContext) /* [SKIPPED] TAPI_1.4.5.8: NEEDS VERIFICATION */
        {
            bool successInjectPlantUndergroundHook = true;
            if (ModContent.GetInstance<WDALQOLServerConfig>().DisableDyePlantGrowthSpeedup)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Strange Plant Underground Hook is disabled in the server configuration, skipping injection...");
                return;
            }
            try
            {
                ILCursor cursor = new ILCursor(intermediateLanguageContext);
                cursor.GotoNext(i => i.MatchLdcI4(2500)); //Move to the position where the number 2500 is pushed onto the stack for RNG.
                cursor.Index++; //Move after it now.
                cursor.Emit(OpCodes.Pop); //Pop Terrarias RNG chance denominator 15000 off the stack.
                int rngDenominator1 = 2; //Set 2 as the denominator for RNG. 1 in 2 chance = 50%
                cursor.Emit(OpCodes.Ldc_I4, rngDenominator1); //Finally, push our denominator onto the stack instead.
                cursor.GotoNext(i => i.MatchLdcI4(10000)); //Move to the position where the number 10000 is pushed onto the stack for RNG.
                cursor.Index++; //Move after it now.
                cursor.Emit(OpCodes.Pop); //Pop Terrarias RNG chance denominator 15000 off the stack.
                int rngDenominator2 = 1; //Set 1 as the denominator for RNG. 1 in 1 chance = 100%
                cursor.Emit(OpCodes.Ldc_I4, rngDenominator2); //Finally, push our denominator onto the stack instead.
            }
            catch
            {
                MonoModHooks.DumpIL(ModContent.GetInstance<WeDoALittleQualityOfLife>(), intermediateLanguageContext);
                WeDoALittleQualityOfLife.logger.Fatal("WDALT: Failed to inject Strange Plant Underground Hook. Broken IL Code has been dumped to tModLoader-Logs/ILDumps/WeDoALittleQualityOfLife.");
                successInjectPlantUndergroundHook = false;
            }
            if(successInjectPlantUndergroundHook)
            {
                WeDoALittleQualityOfLife.logger.Debug("WDALT: Successfully injected Strange Plant Underground Hook via IL Editing.");
            }
        }
    }
}
