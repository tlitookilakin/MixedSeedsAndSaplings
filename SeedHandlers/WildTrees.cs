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

	public static bool IsMixedSapling(bool result, string itemId)
		=> result || itemId == That.item_id;

	public static void ModifySeedIdIfNeeded(ref string itemId)
	{
		itemId = "(O)" + That.ReplaceMixedSeed(itemId);
	}

	public override void Patch(Harmony harmony)
	{
		harmony.Patch(
			typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.isWildTreeSeed)),
			postfix: new(typeof(WildTrees), nameof(IsMixedSapling))
		);

		harmony.Patch(
			typeof(Tree).GetMethod(nameof(Tree.ResolveTreeTypeFromSeed)),
			prefix: new(typeof(WildTrees), nameof(ModifySeedIdIfNeeded))
		);
	}

	protected override IEnumerable<string> GetKeys()
		=> DataLoader.WildTrees(Game1.content).Values.Select(static s => s.SeedItemId);
}
