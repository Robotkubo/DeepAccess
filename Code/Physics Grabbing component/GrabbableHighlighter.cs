using Sandbox;

public sealed class GrabbableHighlighter : Component
{
	[Property, Group( "Highlight" )]
	public Color HighlightColor { get; set; } = new Color( 0.4f, 0.8f, 1f, 0.7f );

	[Property, Group( "Highlight" )]
	public float HighlightStrength { get; set; } = 1.2f;

	private ModelRenderer Renderer;
	private Color OriginalTint;

	protected override void OnStart()
	{
		Renderer = Components.GetInChildren<ModelRenderer>( true );

		if ( Renderer != null )
			OriginalTint = Renderer.Tint;
	}

	public void SetHighlighted( bool highlighted )
	{
		if ( Renderer == null ) return;

		if ( highlighted )
		{
			// Bright cyan/blue highlight
			Renderer.Tint = HighlightColor * HighlightStrength;
		}
		else
		{
			// Restore original color
			Renderer.Tint = OriginalTint;
		}
	}
}
