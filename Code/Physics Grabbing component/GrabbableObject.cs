using Sandbox;

public sealed class GrabbableObject : Component
{
	[Property, Group( "Grab Settings" )]
	public bool CanGrab { get; set; } = true;

	[Property, Group( "Grab Settings" )]
	public float MaxGrabMass { get; set; } = 150f;

	[Property, Group( "Grab Settings" )]
	public bool FreezeRotationOnGrab { get; set; } = false;

	[Property, Group( "Audio" )]
	public SoundEvent GrabSound { get; set; }

	[Property, Group( "Audio" )]
	public SoundEvent DropSound { get; set; }

	public PhysicsBody PhysicsBody => Components.Get<Rigidbody>()?.PhysicsBody;

	private bool IsHeld;

	protected override void OnStart()
	{
		if ( PhysicsBody == null )
		{
			Log.Warning( $"{GameObject.Name} has GrabbableObject but no Rigidbody!" );
		}
	}

	public void OnGrab()
	{
		if ( PhysicsBody == null ) return;

		IsHeld = true;
		PhysicsBody.GravityEnabled = false;
		PhysicsBody.Sleeping = false;

		if ( FreezeRotationOnGrab )
			PhysicsBody.AngularVelocity = Vector3.Zero;

		if ( GrabSound != null )
			Sound.Play( GrabSound, Transform.Position );
	}

	public void OnDrop()
	{
		if ( PhysicsBody == null ) return;

		IsHeld = false;
		PhysicsBody.GravityEnabled = true;

		if ( DropSound != null )
			Sound.Play( DropSound, Transform.Position );
	}

	public void OnThrow( Vector3 force )
	{
		OnDrop();
		PhysicsBody?.ApplyImpulse( force );
	}
}
