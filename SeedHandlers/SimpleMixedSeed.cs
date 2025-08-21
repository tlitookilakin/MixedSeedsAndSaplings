using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;

namespace MixedSeedsAndSaplings.SeedHandlers;

internal abstract class SimpleMixedSeed
{
	public IAssetName KeySource { get; protected set; }

	protected readonly List<string> Keys = [];
	protected readonly string item_id;
	protected readonly IModHelper Helper;

	public virtual void Invalidate()
	{
		Keys.Clear();
	}

	public abstract void Patch(Harmony harmony);

	protected abstract IEnumerable<string> GetKeys();

	public SimpleMixedSeed(IModHelper helper, Harmony harmony, string source, string item)
	{
		Helper = helper;
		item_id = "tlitoo.MixedSeedsAndSaplings_" + item;
		KeySource = helper.GameContent.ParseAssetName(source);
		Patch(harmony);
	}

	public string ReplaceMixedSeed(string existing)
	{
		if (existing != item_id && existing != "(O)" + item_id)
			return existing;

		if (Keys.Count is 0)
			Keys.AddRange(GetKeys());

		return Game1.random.ChooseFrom(Keys);
	}
}
