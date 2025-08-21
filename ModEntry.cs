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
		private static Harmony harmony;

		public override void Entry(IModHelper helper)
		{
			harmony = new(ModManifest.UniqueID);
			Crops.Init(Helper, harmony);
			Assets.Init(helper);

			helper.Events.Content.AssetsInvalidated += Invalidated;
			helper.Events.GameLoop.GameLaunched += Launched;
		}

		private void Launched(object? sender, GameLaunchedEventArgs e)
		{
			SimpleSeeds = [
				new FruitTrees(Helper, harmony),
				new WildTrees(Helper, harmony),
				new Bushes(Helper, harmony)
			];
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
