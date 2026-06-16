using Sandbox;

public sealed class PhysicsGrabber : Component
{
	[Property, Group( "Settings" )] public float GrabDistance { get; set; } = 200f;
	[Property, Group( "Settings" )] public float HoldDistance { get; set; } = 100f;
	[Property, Group( "Settings" )] public float MaxMass { get; set; } = 600f;

	[Property, Group( "Forces" )] public float PullForce { get; set; } = 1200f;
	[Property, Group( "Forces" )] public float Damping { get; set; } = 70f;

	private CameraComponent Camera;
	private PhysicsBody HeldBody;
	private GrabbableObject HeldObject;
	private Vector3 LocalGrabOffset;
	private Rotation StoredRotationOffset;
	private bool RotateMode;

	protected override void OnStart()
	{
		Camera = Components.GetInChildren<CameraComponent>( true ) ?? Scene.Camera;
	}

	protected override void OnUpdate()
	{
		if ( Camera == null ) return;

		if ( Input.Pressed( "use" ) )
		{
			if ( HeldBody == null )
				TryGrab();
			else
				Drop();
		}

		if ( Input.Pressed( "attack2" ) && HeldBody != null )
			Throw();

		RotateMode = Input.Down( "reload" ) && HeldBody != null;

		if ( RotateMode && HeldBody != null )
		{
			StoredRotationOffset = Camera.WorldRotation.Inverse * HeldBody.Rotation;
		}
	}

	protected override void OnFixedUpdate()
	{
		if ( HeldBody == null ) return;
		UpdateHeldObject();
	}

	void TryGrab()
	{
		var start = Camera.WorldPosition;
		var end = start + Camera.WorldRotation.Forward * GrabDistance;

		var tr = Scene.Trace.Ray( start, end )
			.IgnoreGameObject( GameObject.Root )
			.Run();

		if ( !tr.Hit || tr.GameObject == null ) return;

		var grabbable = tr.GameObject.GetComponentInParent<GrabbableObject>( true );
		if ( grabbable == null || !grabbable.CanGrab ) return;

		var body = grabbable.PhysicsBody;
		if ( body == null || body.Mass > MaxMass ) return;

		HeldBody = body;
		HeldObject = grabbable;
		LocalGrabOffset = body.Transform.PointToLocal( tr.HitPosition );

		grabbable.OnGrab();
	}

	void UpdateHeldObject()
	{
		var targetPos = Camera.WorldPosition + Camera.WorldRotation.Forward * HoldDistance;

		var direction = targetPos - HeldBody.Position;

		// Original style forces
		var positionalForce = direction * PullForce;
		var velocityForce = -HeldBody.Velocity * Damping;

		var totalForce = (positionalForce + velocityForce) * HeldBody.Mass;

		// Add a bit of gravity compensation
		totalForce += Vector3.Up * 9.81f * HeldBody.Mass * 0.6f;

		HeldBody.ApplyForce( totalForce );

		HeldBody.AngularVelocity *= 0.82f;
		HeldBody.AngularDamping = 8f;

		// Rotation Mode
		if ( RotateMode )
		{
			var targetRotation = Camera.WorldRotation * StoredRotationOffset;
			var delta = (targetRotation * HeldBody.Rotation.Inverse).Angles();

			var torque = new Vector3( delta.pitch, delta.yaw, delta.roll ) * 35f * HeldBody.Mass;
			HeldBody.ApplyTorque( torque );
		}
	}

	void Drop()
	{
		HeldObject?.OnDrop();
		HeldObject = null;
		HeldBody = null;
	}

	void Throw()
	{
		if ( HeldObject == null ) return;

		var force = Camera.WorldRotation.Forward * 750f * HeldBody.Mass;

		HeldObject.OnThrow( force );

		HeldObject = null;
		HeldBody = null;
	}
}
