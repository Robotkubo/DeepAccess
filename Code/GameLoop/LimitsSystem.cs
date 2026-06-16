using Sandbox.UI;

/// <summary>
/// Stripped-down LimitsSystem for horror game (no spawning limits needed)
/// </summary>
internal sealed class LimitsSystem : GameObjectSystem<LimitsSystem>
{
	public LimitsSystem( Scene scene ) : base( scene )
	{
	}

	// You can add your own limits later (e.g. sanity, entities, etc.)
}
