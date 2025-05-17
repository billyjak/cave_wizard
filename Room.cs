using Godot;
using System;

public partial class Room : Node2D
{
	[Export] public PackedScene EnemyScene { get; set; }
	[Export] public PackedScene PortalScene { get; set; }
	[Export] public int EnemyCount = 3;

	private int enemiesRemaining;

	public override void _Ready()
	{
		enemiesRemaining = EnemyCount;

		for (int i = 0; i < EnemyCount; i++)
		{
			CharacterBody2D enemy = (CharacterBody2D)EnemyScene.Instantiate();
			enemy.Position = new Vector2(GD.Randf() * 800, GD.Randf() * 600);
			enemy.Connect("tree_exited", new Callable(this, nameof(OnEnemyDefeated)));
			AddChild(enemy);
		}
	}

	private void OnEnemyDefeated()
	{
		enemiesRemaining--;
		if (enemiesRemaining <= 0)
		{
			EmitSignal(SignalName.RoomCleared);
			Node2D portal = (Node2D)PortalScene.Instantiate();
			portal.Position = new Vector2(400, 300);
			AddChild(portal);
		}
	}

	[Signal]
	public delegate void RoomClearedEventHandler();
}
