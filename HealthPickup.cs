using Godot;
using System;

public partial class HealthPickup : Area2D
{
	[Export] public int HealAmount = 1;

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			body.Call("HealToFull");
			QueueFree();
		}
	}
}
