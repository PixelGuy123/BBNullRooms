using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BBNullableRooms.Plugin
{
	[BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.bbgenfixes")] // Depends on the Mystery room fix
	public class BasePlugin : BaseUnityPlugin
	{
		void Awake()
		{
			Harmony harmony = new(ModInfo.GUID);
			harmony.PatchAll();
		}

	}


	internal static class ModInfo
	{
		internal const string GUID = "pixelguy.pixelmodding.baldiplus.bbnullrooms";
		internal const string Name = "BB+ Spawnable Null Rooms";
		internal const string Version = "1.0.0";
	}

	[HarmonyPatch(typeof(LevelGenerator))]
	internal class AddNullableRooms
	{
		[HarmonyPatch("StartGenerate")]
		private static void Prefix(LevelGenerator __instance) =>
			i = __instance;

		[HarmonyPatch("Generate", MethodType.Enumerator)]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> AddingNulls(IEnumerable<CodeInstruction> instructions) =>
			new CodeMatcher(instructions)
			.MatchForward(false,
				new CodeMatch(OpCodes.Stfld, name: "<roomCount>"),
				new CodeMatch(OpCodes.Ldloc_2),
				new CodeMatch(OpCodes.Ldc_I4_1),
				new CodeMatch(OpCodes.Call, AccessTools.Method("LevelBuilder:UpdatePotentialRoomSpawns"))) // Goes to this specific line
			.Insert(Transpilers.EmitDelegate(() => i.controlledRNG.NextDouble() > 0.6d ? 0 : 1), // Another mod will fix the Mystery room issue...
			new CodeInstruction(OpCodes.Add)) // This Add to add to the variable duhs
			.InstructionEnumeration();


		static LevelGenerator i;
	}
}
