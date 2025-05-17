using Godot;
using System;

public partial class LightRangeFire : Area2D
{
	[Export] public float Speed = 400f;
	[Export] public float Lifetime = 1.0f;
	[Export] public int Damage = 1;

	public Vector2 Direction = Vector2.Zero;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		GetTree().CreateTimer(Lifetime).Timeout += QueueFree;
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += Direction * Speed * (float)delta;
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("enemies"))
		{
			body.Call("TakeDamage", Damage);
			QueueFree();
		}
	}
}
