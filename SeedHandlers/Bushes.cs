using HarmonyLib;
using MixedSeedsAndSaplings.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Reflection;

namespace MixedSeedsAndSaplings.SeedHandlers;

internal class Bushes : SimpleMixedSeed
{
	private const string BUSH_MOD = "furyx639.CustomBush";
	private const BindingFlags FLAGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

	private static Bushes That = null!;
	private static ICustomBushAPI? CustomBushes;
	private static string? CheckedId;

	public Bushes(IModHelper helper, Harmony harmony) : base(helper, harmony, "furyx639.CustomBush/Data", "Shrub")
	{
		That = this;
		if (helper.ModRegistry.IsLoaded(BUSH_MOD))
			CustomBushes = helper.ModRegistry.GetApi<ICustomBushAPI>(BUSH_MOD);

		helper.Events.GameLoop.UpdateTicked += Ticked;
	}

	private void Ticked(object? sender, UpdateTickedEventArgs e)
	{
		CheckedId = null;
	}

	public override void Patch(Harmony harmony)
	{
		harmony.Patch(
			typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.IsTeaSapling)),
			postfix: new(typeof(Bushes), nameof(IsBushSapling))
		);

		if (!Helper.ModRegistry.IsLoaded(BUSH_MOD))
			return;

		// custom bush patches
		var asm = Helper.ModRegistry.GetApi(BUSH_MOD)!.GetType().Assembly;
		var patcher = asm.GetType("LeFauxMods.CustomBush.Services.ModPatches")!;

		harmony.Patch(
			patcher.GetMethod("GameLocation_CheckItemPlantRules_postfix", FLAGS),
			prefix: new(typeof(Bushes), nameof(ReplaceId))
		);

		harmony.Patch(
			patcher.GetMethod("IndoorPot_performObjectDropInAction_postfix", FLAGS),
			prefix: new(typeof(Bushes), nameof(ReplaceItem))
		);

		harmony.Patch(
			patcher.GetMethod("Object_placementAction_AddModData", FLAGS),
			prefix: new(typeof(Bushes), nameof(ReplaceObject))
		);
	}

	protected override IEnumerable<string> GetKeys()
	{
		return CustomBushes is null ? ["(O)251"] : CustomBushes.GetAllBushes().Select(static b => b.Id).Append("(O)251");
	}

	private static bool IsBushSapling(bool result, StardewValley.Object __instance)
		=> result || __instance.QualifiedItemId == "(O)" + That.item_id;

	private static void ReplaceId(ref string itemId)
	{
		CheckedId = itemId = That.ReplaceMixedSeed(itemId);
	}

	private static void ReplaceItem(ref Item dropInItem)
	{
		if (CheckedId is null || dropInItem.QualifiedItemId != "(O)" + That.item_id)
			return;

		dropInItem = ItemRegistry.Create(CheckedId);
	}

	private static void ReplaceObject(ref StardewValley.Object obj)
	{
		if (CheckedId is null || obj.QualifiedItemId != "(O)" + That.item_id)
			return;

		obj = ItemRegistry.Create<StardewValley.Object>(CheckedId);
	}
}
