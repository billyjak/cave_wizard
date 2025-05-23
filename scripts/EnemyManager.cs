using Godot;
using System;

public partial class EnemyManager : Node2D
{
	private PackedScene _goblinScene = GD.Load<PackedScene>("res://scenes/goblin_fighter.tscn");

	public void InitializeRoom(Node2D room)
	{
		// Clear existing enemies
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		// Find enemy spawn points in the room
		var spawnPoints = room.GetNodeOrNull<Node>("EnemySpawnPoints");
		if (spawnPoints != null)
		{
			foreach (Node2D spawnPoint in spawnPoints.GetChildren())
			{
				SpawnGoblin(spawnPoint.GlobalPosition);
			}
		}
	}

	private void SpawnGoblin(Vector2 position)
	{
		var goblin = _goblinScene.Instantiate<GoblinFighter>();
		goblin.Position = position;
		AddChild(goblin); // Add to EnemyManager
	}
}
