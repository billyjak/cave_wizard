using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	private const float Speed = 200f;
	private const float DashSpeed = 1000f;

	private PackedScene LightRangeFire = GD.Load<PackedScene>("res://attacks/light/LightRangeFire.tscn");
	private PackedScene LightMeleeFire = GD.Load<PackedScene>("res://attacks/light/LightMeleeFire.tscn");

	private bool creatingSlime = false;
	private bool isDashing = false;
	private bool canDash = true;
	private Vector2 dashDirection = Vector2.Zero;
	private Vector2 lastDirection = Vector2.Right;
	private Node lastFacingSprite = null;
	private List<Node> activeSlimes = new();
	private int health = 5;
	private Label hpLabel = null;

	public override void _Ready()
	{
		GetNode<Sprite2D>("SideSprite").Visible = true;
		lastFacingSprite = GetNode("SideSprite");
		hpLabel = GetTree().CurrentScene.GetNode<Label>("UI/HPLabel");
		UpdateHpLabel();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputAimVector = new(
			Input.GetActionStrength("aim_right") - Input.GetActionStrength("aim_left"),
			Input.GetActionStrength("aim_down") - Input.GetActionStrength("aim_up")
		);

		Vector2 inputDirectionVector = new(
			Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
			Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up")
		);

		HandleDash(inputDirectionVector);
		HandleMovement(inputDirectionVector);
		HandleAttack(inputAimVector, inputDirectionVector);
		HandleSprite(inputDirectionVector);
		MoveAndSlide();
	}

	private void UpdateHpLabel()
	{
		if (hpLabel != null)
		{
			hpLabel.Text = $"HP: {health}";
		}
	}

	public void TakeDamage(int damage)
	{
		health -= damage;
		UpdateHpLabel();
		if (health <= 0)
		{
			CallDeferred("queue_free");
		}
	}

	public void HealToFull()
	{
		health = 5;
		UpdateHpLabel();
	}

	private void HandleMovement(Vector2 inputDirectionVector)
	{
		if (isDashing)
		{
			Velocity = dashDirection * DashSpeed;
		}
		else
		{
			Velocity = inputDirectionVector.Normalized() * Speed;
		}
	}

	private void HandleDash(Vector2 inputDirectionVector)
	{
		if (Input.IsActionJustPressed("dash") && canDash && inputDirectionVector.Length() > 0)
		{
			isDashing = true;
			CreateTimer(0.2f, true, true, () => isDashing = false);
			canDash = false;
			CreateTimer(1.0f, true, true, () => canDash = true);
			dashDirection = inputDirectionVector.Normalized();
		}
	}

	private void HandleAttack(Vector2 inputAimVector, Vector2 inputDirectionVector)
	{
		Vector2 direction = inputDirectionVector.Normalized();
		Vector2 aimDirection = inputAimVector.Normalized();

		if (inputDirectionVector.Length() > 0)
			lastDirection = direction;

		if (Input.IsActionJustPressed("attack_light"))
		{
			if (inputAimVector.Length() > 0)
			{
				var attack = (Node2D)LightRangeFire.Instantiate();
				attack.Rotation = aimDirection.Angle() + Mathf.DegToRad(270);
				attack.GlobalPosition = GlobalPosition + aimDirection * 50;
				attack.Set("direction", aimDirection);
				GetTree().CurrentScene.AddChild(attack);
			}
			else
			{
				var attack = (Node2D)LightMeleeFire.Instantiate();
				Vector2 pos = inputDirectionVector.Length() == 0 ? lastDirection : direction;
				attack.GlobalPosition = GlobalPosition + pos * 100;
				attack.Rotation = pos.Angle() + Mathf.DegToRad(270);
				GetTree().CurrentScene.AddChild(attack);
			}
		}
	}

	private void HandleSprite(Vector2 inputDirectionVector)
	{
		var side = GetNode<Sprite2D>("SideSprite");
		var front = GetNode<Sprite2D>("FrontSprite");
		var back = GetNode<Sprite2D>("BackSprite");

		side.Visible = false;
		front.Visible = false;
		back.Visible = false;

		if (Mathf.Abs(inputDirectionVector.Y) > Mathf.Abs(inputDirectionVector.X))
		{
			if (inputDirectionVector.Y > 0)
			{
				front.Visible = true;
				lastFacingSprite = front;
			}
			else
			{
				back.Visible = true;
				lastFacingSprite = back;
			}
		}
		else
		{
			if (inputDirectionVector.X != 0)
			{
				side.Visible = true;
				side.FlipH = inputDirectionVector.X > 0;
				lastFacingSprite = side;
			}
		}

		if (inputDirectionVector.Length() == 0 && lastFacingSprite is Sprite2D sprite)
		{
			sprite.Visible = true;
		}
	}

	private Timer CreateTimer(float wait, bool oneShot = true, bool autoStart = true, Action? handler = null)
	{
		var timer = new Timer
		{
			WaitTime = wait,
			OneShot = oneShot,
			Autostart = autoStart
		};
		AddChild(timer);
		if (handler != null)
			timer.Timeout += handler;
		return timer;
	}
}
