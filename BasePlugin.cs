using BepInEx;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BBNullableRooms.Plugin
{
	[BepInPlugin(ModInfo.GUID, ModInfo.Name, ModInfo.Version)]
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
			.MatchForward(true, // Why did I needed true? Idk, but it works
				new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(LevelBuilder), "rooms")),
				new CodeMatch(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(List<RoomController>), "Count")),
				new CodeMatch(OpCodes.Add),
				new CodeMatch(OpCodes.Stfld, name:"<roomCount>5__44")
				) // Goes to a specific line
			.Insert(Transpilers.EmitDelegate(() => i.controlledRNG.NextDouble() > 0.6d ? 0 : 1), // Another mod will fix the Mystery room issue...
			new CodeInstruction(OpCodes.Add)) // This Add to add to the variable duhs
			.InstructionEnumeration();


		static LevelGenerator i;
	}
}
