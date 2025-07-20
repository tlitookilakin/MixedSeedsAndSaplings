using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;

namespace MixedSeedsAndSaplings
{
	public class ModEntry : Mod
	{
		private static readonly List<KeyValuePair<string, CropData>> seeds_flowers = [];
		private static readonly List<KeyValuePair<string, CropData>> seeds_other = [];

		public override void Entry(IModHelper helper)
		{
			Harmony harmony = new(ModManifest.UniqueID);
			harmony.Patch(
				typeof(Crop).GetMethod(nameof(Crop.ResolveSeedId)),
				postfix: new(typeof(ModEntry), nameof(ModifySeedId))
			);

			helper.Events.Content.AssetsInvalidated += Invalidated;
		}

		private void Invalidated(object? sender, AssetsInvalidatedEventArgs e)
		{
			foreach(var name in e.NamesWithoutLocale)
			{
				if (name.IsEquivalentTo("Data/Crops"))
				{
					seeds_flowers.Clear();
					seeds_other.Clear();
				}
			}
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
}
