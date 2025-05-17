using Godot;
using System;

public partial class LightMeleeFire : Area2D
{
	[Export] public float Lifetime = 0.15f;
	[Export] public int Damage = 1;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
		GetTree().CreateTimer(Lifetime).Timeout += QueueFree;
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("enemies"))
		{
			body.Call("TakeDamage", Damage);
		}
	}
}
