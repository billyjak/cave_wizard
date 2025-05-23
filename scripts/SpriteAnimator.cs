using Godot;

public partial class SpriteAnimator : Node
{
	[Export]
	private AnimatedSprite2D _animatedSprite; // Export to allow setting in the editor

	public void UpdateDirection(Vector2 velocity)
	{
		if (_animatedSprite == null)
		{
			GD.PrintErr("AnimatedSprite2D is not assigned in SpriteAnimator!");
			return;
		}

		string anim = velocity switch
		{
			{ X: 0, Y: 0 } => "idle",
			{ X: var x, Y: var y } when System.Math.Abs(x) > System.Math.Abs(y) => "side",
			{ Y: < 0 } => "up",
			_ => "down"
		};

		if (anim == "idle") { _animatedSprite.Stop(); return; }
		_animatedSprite.FlipH = anim == "side" && velocity.X < 0;
		_animatedSprite.Animation = anim;
		_animatedSprite.Play();
	}
}
