extends CharacterBody2D
class_name SlimyBoi

signal died(slime: Node)

var health := 3
var speed := 100
var slime_strength := 1
var player_ref = null

static func create(
	player_reference: Node,
	slime_position: Vector2,
	health_val: int = 3,
	speed_val: int = 100,
	strength_val: int = 1
) -> SlimyBoi:
	var slime = preload("res://enemies/SlimyBoi.tscn").instantiate()
	slime.player_ref = player_reference
	slime.global_position = slime_position
	slime.health = health_val
	slime.speed = speed_val
	slime.slime_strength = strength_val
	return slime

func _ready():
	$HitboxArea.connect("body_entered", Callable(self, "_on_body_entered"))
	add_to_group("enemies")

func take_damage(damage):
	health -= damage
	if health <= 0:
		emit_signal("died", self)
		queue_free()

func _on_body_entered(body):
	if body.name == "Player":
		body.take_damage(slime_strength)

func _physics_process(_delta: float) -> void:
	if player_ref and is_instance_valid(player_ref):
		var direction = (player_ref.global_position - global_position).normalized()
		velocity = direction * speed
		move_and_slide()
