using Sandbox;

public class ControlSystem : GameObjectSystem<ControlSystem>
{
	public ControlSystem( Scene scene ) : base( scene )
	{
	}

	// This system was mainly used for vehicles, chairs, and contraptions in Sandbox.
	// We keep it alive for basic sitting support, but stripped of editor-heavy features.
}
