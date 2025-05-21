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

		if (velocity.Length() == 0)
		{
			_animatedSprite.Stop();
			return;
		}

		if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
		{
			_animatedSprite.Animation = "side";
			_animatedSprite.FlipH = velocity.X < 0;
		}
		else if (velocity.Y < 0)
		{
			_animatedSprite.Animation = "up";
		}
		else
		{
			_animatedSprite.Animation = "down";
		}

		_animatedSprite.Play();
	}
}
