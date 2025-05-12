extends CharacterBody2D

@export var lifetime := 1
@export var damage := 1
var direction = Vector2.ZERO


const SPEED = 1000


func _ready():
	$HitboxArea.connect("body_entered", Callable(self, "_on_body_entered"))
	await get_tree().create_timer(lifetime).timeout
	queue_free()

func _on_body_entered(body):
	if body.is_in_group("enemies"):
		body.take_damage(damage)
		queue_free()
		

	
func _physics_process(_delta: float) -> void:

	velocity = direction.normalized() * SPEED
	move_and_slide()
