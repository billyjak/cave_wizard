extends Area2D
signal portal_entered

func _ready():
	connect("body_entered", Callable(self, "_on_body_entered"))

func _on_body_entered(body):
	if body.is_in_group("Player"):
		call_deferred("emit_signal", "portal_entered")
		call_deferred("queue_free")
