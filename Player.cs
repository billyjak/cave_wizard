using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public int Speed = 200;

	private AnimatedSprite2D anim;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputVector = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

		Velocity = inputVector * Speed;
		MoveAndSlide();

		UpdateAnimationDirection(Velocity);
	}

	private void UpdateAnimationDirection(Vector2 velocity)
	{
		if (velocity.Length() == 0)
		{
			anim.Stop();
			return;
		}

		if (Mathf.Abs(velocity.X) > Mathf.Abs(velocity.Y))
		{
			anim.Animation = "side";
			anim.FlipH = velocity.X < 0;
		}
		else if (velocity.Y < 0)
		{
			anim.Animation = "up";
		}
		else
		{
			anim.Animation = "down";
		}

		anim.Play();
	}
}
