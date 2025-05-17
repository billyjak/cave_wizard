using Godot;
using System;

public partial class Portal : Area2D
{
	[Export] public PackedScene NextRoomScene { get; set; }

	private void OnBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			Node2D nextRoom = (Node2D)NextRoomScene.Instantiate();
			GetTree().CurrentScene.CallDeferred("add_child", nextRoom);
			QueueFree();
		}
	}

	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}
}
