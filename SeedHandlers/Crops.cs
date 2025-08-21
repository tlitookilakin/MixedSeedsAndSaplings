using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;

namespace MixedSeedsAndSaplings.SeedHandlers;

internal class Crops
{
	private static readonly List<KeyValuePair<string, CropData>> seeds_flowers = [];
	private static readonly List<KeyValuePair<string, CropData>> seeds_other = [];

	public static IAssetName Watched { get; private set; } = null!;

	public static void Init(IModHelper helper, Harmony harmony)
	{
		harmony.Patch(
				typeof(Crop).GetMethod(nameof(Crop.ResolveSeedId)),
				postfix: new(typeof(Crops), nameof(ModifySeedId))
			);

		Watched = helper.GameContent.ParseAssetName("Data/Crops");
	}

	public static void Invalidate()
	{
		seeds_flowers.Clear();
		seeds_other.Clear();
	}

	private static string ModifySeedId(string seed, string itemId, GameLocation location)
	{
		if (itemId is "MixedFlowerSeeds")
			return GetRandomSeed(location, true);
		if (itemId is "770")
			return GetRandomSeed(location, false);

		return seed;
	}

	private static string GetRandomSeed(GameLocation where, bool flowers)
	{
		UpdateSeedCache();
		var s = where.GetSeason();

		var options = flowers ? seeds_flowers : seeds_other;
		if (!where.SeedsIgnoreSeasonsHere())
			options = [.. from p in options where p.Value.Seasons.Contains(s) select p];
		return Game1.random.ChooseFrom(options).Key;
	}

	private static void UpdateSeedCache()
	{
		if (seeds_other.Count is not 0)
			return;

		seeds_flowers.Clear();

		foreach (var pair in DataLoader.Crops(Game1.content))
		{
			if (ItemContextTagManager.HasBaseTag(pair.Value.HarvestItemId, "category_flowers"))
				seeds_flowers.Add(pair);
			else
				seeds_other.Add(pair);
		}
	}
}
