namespace MixedSeedsAndSaplings.APIs;

public interface ICustomBushAPI
{
	/// <summary>Retrieves all the custom bush data.</summary>
	/// <returns>Each object represents an instance of the <see cref="ICustomBushData" /> model.</returns>
	public IEnumerable<ICustomBushData> GetAllBushes();
}

public interface ICustomBushData
{
	/// <summary>Gets a unique identifier for the custom bush.</summary>
	public string Id { get; }
}