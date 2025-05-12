extends CharacterBody2D

@export var lifetime := 1
@export var damage := 1
var direction = Vector2.ZERO


const SPEED = 1000


func _ready():
	$HitboxArea.connect("body_entered", Callable(self, "_on_body_entered"))
	connect("body_entered", Callable(self, "_on_body_entered"))
	await get_tree().create_timer(lifetime).timeout
	queue_free()

func _on_body_entered(body):
	if body.is_in_group("enemies"):
		body.take_damage(damage)
		queue_free()
		

	
func _physics_process(_delta: float) -> void:


	# var input_aim_vector = Vector2(
	# 	Input.get_action_strength("aim_right") - Input.get_action_strength("aim_left"),
	# 	Input.get_action_strength("aim_down") - Input.get_action_strength("aim_up")
	# 	)

	velocity = direction.normalized() * SPEED
	move_and_slide()
