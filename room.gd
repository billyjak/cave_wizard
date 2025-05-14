extends Node2D

const HEART_SCENE = preload("res://HealthPickup.tscn")
const PORTAL_SCENE = preload("res://Portal.tscn")
const ROOM_SCENE = preload("res://Room.tscn")


var player

var enemies = []

func _ready() -> void:
	call_deferred("_init_room")


func _init_room():

	var players = get_tree().get_nodes_in_group("Player")
	if players.size() == 1:
		player = players[0]
	elif players.size() == 0:
		print("❌ No player found.")
	else:
		print("⚠️ Multiple players found — using the first one.")
	player = players[0]
	spawn_enemies(5)


func spawn_enemies(count: int):
	for i in count:
		var pos = Vector2(
		randf_range(-500, 500),
		randf_range(-500, 500)
		)
		var slime = SlimyBoi.create(player, player.position + pos, 3, 100, 1)
		add_child(slime)
		enemies.append(slime)
		slime.connect("died", Callable(self, "_on_enemy_died"))

func _spawn_heart():
	var heart = HEART_SCENE.instantiate()
	heart.position = Vector2(400,300)
	add_child(heart)

func _spawn_portal():
	var portal = PORTAL_SCENE.instantiate()
	portal.position = Vector2(900, 300)
	add_child(portal)
	portal.connect("portal_entered", Callable(self, "_on_portal_entered"))

func _on_portal_entered():
	var room = ROOM_SCENE.instantiate()
	get_parent().add_child(room)
	queue_free()

func _on_enemy_died(enemy):
	enemies.erase(enemy)
	if enemies.is_empty():
		print("room is clear innit m8")
		call_deferred("_spawn_heart")
		call_deferred("_spawn_portal")
