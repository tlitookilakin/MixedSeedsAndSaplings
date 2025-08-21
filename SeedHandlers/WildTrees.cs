using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace MixedSeedsAndSaplings.SeedHandlers;

internal class WildTrees : SimpleMixedSeed
{
	private static WildTrees That = null!;

	public WildTrees(IModHelper helper, Harmony harmony) : base(helper, harmony, "Data/WildTrees", "WildSapling")
	{
		That = this;
	}

	public static bool IsMixedSapling(bool result, StardewValley.Object __instance)
		=> result || __instance.QualifiedItemId == "(O)" + That.item_id;

	public static void ModifySeedIdIfNeeded(ref string id)
	{
		id = That.ReplaceMixedSeed(id);
	}

	public override void Patch(Harmony harmony)
	{
		harmony.Patch(
			typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.IsWildTreeSapling)),
			postfix: new(typeof(WildTrees), nameof(IsMixedSapling))
		);

		harmony.Patch(
			typeof(Tree).GetConstructor([typeof(string), typeof(int), typeof(bool)]),
			prefix: new(typeof(WildTrees), nameof(ModifySeedIdIfNeeded))
		);
	}

	protected override IEnumerable<string> GetKeys()
		=> DataLoader.WildTrees(Game1.content).Keys;
}
