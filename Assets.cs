using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;

namespace MixedSeedsAndSaplings;

internal class Assets
{
	private static IAssetName ItemData = null!;
	private static IAssetName ItemSheet = null!;
	private static IAssetName Strings = null!;
	private static IModContentHelper ModContent = null!;
	private static Func<Dictionary<string, string>> GetStrings = null!;

	public static void Init(IModHelper helper)
	{
		ItemData = helper.GameContent.ParseAssetName("Data/Objects");
		ItemSheet = helper.GameContent.ParseAssetName("Mods/tlitoo.MixedSeedsAndSaplings/Items");
		Strings = helper.GameContent.ParseAssetName("Mods/tlitoo.MixedSeedsAndSaplings/Strings");

		ModContent = helper.ModContent;
		GetStrings = () => helper.Translation.GetTranslations().ToDictionary(static t => t.Key, static t => (string)t);

		helper.Events.Content.AssetRequested += Requested;
	}

	private static void Requested(object? sender, AssetRequestedEventArgs e)
	{
		if (e.Equals(ItemData))
			e.Edit(AddItems);
		else if (e.Equals(ItemSheet))
			e.LoadFromModFile<Texture2D>("assets/items.png", AssetLoadPriority.Medium);
		else if (e.Equals(Strings))
			e.LoadFrom(GetStrings, AssetLoadPriority.Medium);
	}

	private static void AddItems(IAssetData data)
	{
		if (data.Data is not IDictionary<string, ObjectData> objs)
			return;

		foreach ((var key, var val) in ModContent.Load<IDictionary<string, ObjectData>>("assets/items.json"))
		{
			val.Texture = ItemSheet.ToString();
			val.Description = $"[LocalizedText Mods/tlitoo.MixedSeedsAndSaplings/Strings:items.{key}.desc]";
			val.DisplayName = $"[LocalizedText Mods/tlitoo.MixedSeedsAndSaplings/Strings:items.{key}.name]";
			objs["tlitoo.MixedSeedsAndSaplings_" + key] = val;
		}
	}
}
