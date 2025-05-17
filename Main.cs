using Godot;
using System;

public partial class Main : Node2D
{
	[Export] public PackedScene RoomScene { get; set; }

	public override void _Ready()
	{
		SpawnRoom();
	}

	private void SpawnRoom()
	{
		if (RoomScene != null)
		{
			Node2D room = (Node2D)RoomScene.Instantiate();
			AddChild(room);
		}
	}
}
