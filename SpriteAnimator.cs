using Godot;
using System;

public partial class SpriteAnimator : Node
{
	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
		animatedSprite = GetParent().GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public void UpdateDirection(Vector2 velocity)
	{
		if (animatedSprite == null) return;

		if (velocity.Length() == 0)
		{
			animatedSprite.Stop();
			return;
		}

		if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
		{
			animatedSprite.Animation = "side";
			animatedSprite.FlipH = velocity.X < 0;
		}
		else if (velocity.Y < 0)
		{
			animatedSprite.Animation = "up";
		}
		else
		{
			animatedSprite.Animation = "down";
		}

		animatedSprite.Play();
	}
}
