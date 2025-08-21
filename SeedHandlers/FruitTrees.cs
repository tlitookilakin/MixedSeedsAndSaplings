using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.TerrainFeatures;

namespace MixedSeedsAndSaplings.SeedHandlers;

internal class FruitTrees : SimpleMixedSeed
{
	private static FruitTrees That = null!;

	public FruitTrees(IModHelper helper, Harmony harmony) : base(helper, harmony, "Data/FruitTrees", "FruitSapling")
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
			typeof(StardewValley.Object).GetMethod(nameof(StardewValley.Object.IsFruitTreeSapling)),
			postfix: new(typeof(FruitTrees), nameof(IsMixedSapling))
		);

		harmony.Patch(
			typeof(FruitTree).GetConstructor([typeof(string), typeof(int)]),
			prefix: new(typeof(FruitTrees), nameof(ModifySeedIdIfNeeded))
		);
	}

	protected override IEnumerable<string> GetKeys()
		=> DataLoader.FruitTrees(Game1.content).Keys;
}