using Godot;
using System;

public partial class LevelManager : Node
{
    private Node2D _currentRoom;
    private int _currentRoomIndex = 1; // Start at room_001
    private const string RoomPathFormat = "res://rooms/room_{0:D3}.tscn"; // e.g., room_001.tscn
    private Node2D _main; // Reference to Main node
    private Node2D _player; // Reference to Player node
    private EnemyManager _enemyManager; // Reference to EnemyManager

    public override void _Ready()
    {
        _main = GetTree().Root.GetNode<Node2D>("/root/Main");
        _player = _main.GetNode<Node2D>("Player");
        _enemyManager = _main.GetNode<EnemyManager>("EnemyManager");
        LoadRoom(_currentRoomIndex);
    }

    public void LoadRoom(int roomIndex)
    {
        // Unload current room if exists
        if (_currentRoom != null)
        {
            _currentRoom.QueueFree();
            _currentRoom = null;
        }

        // Load new room
        string roomPath = string.Format(RoomPathFormat, roomIndex);
        if (!ResourceLoader.Exists(roomPath))
        {
            GD.PrintErr($"Room not found: {roomPath}");
            return;
        }

        var roomScene = GD.Load<PackedScene>(roomPath);
        _currentRoom = roomScene.Instantiate<Node2D>();
        _currentRoom.Name = "CurrentRoom";
        _main.AddChild(_currentRoom);

        // Move player to room's spawn point (if defined)
        var spawnPoint = _currentRoom.GetNodeOrNull<Node2D>("SpawnPoint");
        if (spawnPoint != null)
        {
            _player.GlobalPosition = spawnPoint.GlobalPosition;
        }

        // Initialize enemies
        _enemyManager.InitializeRoom(_currentRoom);

        _currentRoomIndex = roomIndex;
    }

    public void LoadNextRoom()
    {
        LoadRoom(_currentRoomIndex + 1);
    }

    public void LoadPreviousRoom()
    {
        LoadRoom(_currentRoomIndex - 1);
    }

    // Getters for other scripts to access player and enemy manager
    public Node2D GetPlayer() => _player;
    public EnemyManager GetEnemyManager() => _enemyManager;
}
