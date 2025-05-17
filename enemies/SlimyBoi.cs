using Godot;
using System;

public partial class SlimyBoi : CharacterBody2D
{
    [Export] public int Speed = 100;
    private Node2D player;
    private int health = 3;

    public override void _Ready()
    {
        player = GetTree().CurrentScene.GetNode<Node2D>("Player");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (player != null)
        {
            Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
            Velocity = direction * Speed;
            MoveAndSlide();
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            QueueFree();
        }
    }
}