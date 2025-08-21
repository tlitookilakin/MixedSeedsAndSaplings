using HarmonyLib;
using MixedSeedsAndSaplings.SeedHandlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.Crops;

namespace MixedSeedsAndSaplings
{
	public class ModEntry : Mod
	{
		private static List<SimpleMixedSeed> SimpleSeeds = [];

		public override void Entry(IModHelper helper)
		{
			Harmony harmony = new(ModManifest.UniqueID);

			Crops.Init(helper, harmony);
			SimpleSeeds = [
				new FruitTrees(helper, harmony),
				new WildTrees(helper, harmony)
			];
			Assets.Init(helper);

			helper.Events.Content.AssetsInvalidated += Invalidated;
		}

		private void Invalidated(object? sender, AssetsInvalidatedEventArgs e)
		{
			foreach(var name in e.NamesWithoutLocale)
			{
				if (name.Equals(Crops.Watched)) 
				{
					Crops.Invalidate();
					continue;
				}

				foreach (var type in SimpleSeeds)
				{
					if (type.KeySource.Equals(name))
					{
						type.Invalidate();
						break;
					}
				}
			}
		}
	}
}
