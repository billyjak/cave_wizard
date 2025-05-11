extends Area2D

@export var lifetime := .15
@export var damage := 1


func _ready():
	connect("body_entered", Callable(self, "_on_body_entered"))
	await get_tree().create_timer(lifetime).timeout
	queue_free()

func _on_body_entered(body):
	if body.is_in_group("enemies"):
		body.take_damage(damage)
