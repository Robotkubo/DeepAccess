public sealed partial class GameManager
{
	[ConCmd( "spawn" )]
	private static void SpawnCommand( string ident ) { }

	[Rpc.Broadcast]
	public static async void Spawn( string ident, string metadata = null, bool forceWorld = false ) { }
}
