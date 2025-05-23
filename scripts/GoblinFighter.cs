using Godot;
using System;

public partial class GoblinFighter : CharacterBody2D
{
	[Export]
	private SpriteAnimator _spriteAnimator;
	[Export]
	private NavigationAgent2D _navigationAgent;
	[Export]
	public int Speed = 100;
	[Export]
	public float ChaseDistance = 300f;
	[Export]
	public float AttackDistance = 50f;
	[Export]
	public float Health = 100f;
	[Export]
	public float LowHealthThreshold = 30f;

	private enum State { Idle, Chase, Attack, Flee }
	private State _currentState = State.Idle;
	private Vector2 _fleePoint;
	private float _stateTimer = 0f;
	private Node2D _player;

	public override void _Ready()
	{
		if (_navigationAgent == null)
		{
			GD.PrintErr("NavigationAgent2D is not assigned in GoblinFighter!");
		}
		_navigationAgent.PathDesiredDistance = 10f;
		_navigationAgent.TargetDesiredDistance = 10f;
		_fleePoint = GlobalPosition + new Vector2(GD.Randf() * 200 - 100, GD.Randf() * 200 - 100);

		_player = GetNode<LevelManager>("/root/LevelManager").GetPlayer(); // Use singleton instance
		if (_player == null)
		{
			GD.PrintErr("Player not found in GoblinFighter!");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player == null || _navigationAgent == null)
		{
			return;
		}

		UpdateState(delta);
		Velocity = CalculateVelocity();
		MoveAndSlide();
		_spriteAnimator?.UpdateDirection(Velocity);
	}

	private void UpdateState(double delta)
	{
		float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);
		_stateTimer -= (float)delta;

		_currentState = (Health, distanceToPlayer, _stateTimer) switch
		{
			var (h, _, _) when h <= LowHealthThreshold => State.Flee,
			var (_, d, t) when d < AttackDistance && t <= 0 => State.Attack,
			var (_, d, _) when d < ChaseDistance => State.Chase,
			_ => State.Idle
		};

		if (_currentState == State.Attack)
		{
			_stateTimer = 1f;
		}
	}

	private Vector2 CalculateVelocity()
	{
		Vector2 desiredVelocity = Vector2.Zero;

		switch (_currentState)
		{
			case State.Chase:
				_navigationAgent.TargetPosition = _player.GlobalPosition;
				if (!_navigationAgent.IsNavigationFinished())
				{
					Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
					desiredVelocity = (nextPathPosition - GlobalPosition).Normalized() * Speed;
					desiredVelocity = ApplyObstacleAvoidance(desiredVelocity);
				}
				break;

			case State.Attack:
				desiredVelocity = Vector2.Zero;
				GD.Print("Goblin attacking!");
				break;

			case State.Flee:
				_navigationAgent.TargetPosition = _fleePoint;
				if (!_navigationAgent.IsNavigationFinished())
				{
					Vector2 nextPathPosition = _navigationAgent.GetNextPathPosition();
					desiredVelocity = (nextPathPosition - GlobalPosition).Normalized() * Speed;
				}
				else
				{
					_fleePoint = GlobalPosition + new Vector2(GD.Randf() * 200 - 100, GD.Randf() * 200 - 100);
				}
				break;

			case State.Idle:
				desiredVelocity = Vector2.Zero;
				break;
		}

		return desiredVelocity;
	}

	private Vector2 ApplyObstacleAvoidance(Vector2 desiredVelocity)
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + desiredVelocity.Normalized() * 50f);
		var result = spaceState.IntersectRay(query);

		if (result.Count > 0)
		{
			Vector2 normal = (Vector2)result["normal"];
			desiredVelocity = (desiredVelocity + normal * 50f).Normalized() * Speed;
		}

		return desiredVelocity;
	}

	public void TakeDamage(float damage)
	{
		Health -= damage;
		if (Health <= 0)
		{
			QueueFree();
		}
	}
}
