extends CharacterBody2D

var health := 3
var speed := 100
var player_ref = null
var slime_strength = 1


func _ready():
	$HitboxArea.connect("body_entered", Callable(self, "_on_body_entered"))
	add_to_group("enemies")

func take_damage(damage):
	health -= damage
	print("slimey boy took damage. health is now: " + str(health))
	if health <= 0:
		queue_free()

func _on_body_entered(body):
	# print("player has entered body")
	if body.name == "Player":
		body.take_damage(slime_strength)

func _physics_process(_delta: float) -> void:
	if player_ref and is_instance_valid(player_ref):
		var direction = (player_ref.global_position - global_position).normalized()
		velocity = direction * speed
		move_and_slide()
