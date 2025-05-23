using Godot;

public partial class Player : CharacterBody2D
{
	[Export]
	private SpriteAnimator _spriteAnimator;
	[Export]
	public int Speed = 200;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputVector = new Vector2(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		).Normalized();

		Velocity = inputVector * Speed;
		MoveAndSlide();
		_spriteAnimator?.UpdateDirection(Velocity);
	}
}
