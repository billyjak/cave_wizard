extends CharacterBody2D

const SPEED = 200
const DASH_SPEED = 1000
const LightRangeFire = preload("res://attacks/light/LightRangeFire.tscn")
const LightMeleeFire = preload("res://attacks/light/LightMeleeFire.tscn")

var creating_slime := false
var is_dashing := false
var can_dash := true
var dash_direction := Vector2.ZERO
var last_direction := Vector2.RIGHT
var last_facing_sprite: Node = null
var active_slimes := []
var health := 5
var hp_label = null

func update_hp_label():
	if hp_label:
		hp_label.text = "HP: " + str(health)

func take_damage(damage: int):
	health -= damage
	update_hp_label()
	if health <= 0:
		queue_free()

func slimy_boi_spawner():
	if active_slimes.size() == 0 and creating_slime == false:
		creating_slime = true
		create_timer(2.0, true, true, func():
			var slime = SlimyBoi.create(self, get_viewport_rect().size / 2, 3, 100, 1)
			get_tree().current_scene.add_child(slime)
			active_slimes.append(slime)
			slime.connect("tree_exited", Callable(self, "_on_slime_exited").bind(slime))
		)

func _on_slime_exited(slime):
	active_slimes.erase(slime)
	creating_slime = false

func handle_movement(input_direction_vector: Vector2):
	if is_dashing:
		velocity = dash_direction * DASH_SPEED	
	else:
		velocity = input_direction_vector.normalized() * SPEED

func handle_dash(input_direction_vector: Vector2):
	if Input.is_action_just_pressed("dash") and can_dash and input_direction_vector.length() > 0:
		is_dashing = true
		create_timer(0.2, true, true, func(): is_dashing = false)
		can_dash = false
		create_timer(1.0, true, true, func(): can_dash = true)
		dash_direction = input_direction_vector.normalized()

func handle_attack(input_aim_vector: Vector2, input_direction_vector: Vector2):
	var direction = input_direction_vector.normalized()
	var aim_direction = input_aim_vector.normalized()
	if input_direction_vector.length() > 0:
		last_direction = direction
	if Input.is_action_just_pressed("attack_light"):
		if input_aim_vector.length() > 0:
			var attack = LightRangeFire.instantiate()
			attack.rotation = aim_direction.angle() + deg_to_rad(270)
			attack.global_position = global_position + aim_direction * 50
			attack.direction = aim_direction
			get_tree().current_scene.add_child(attack)
		else:
			var attack = LightMeleeFire.instantiate()
			if input_direction_vector.length() == 0:
				attack.global_position = global_position + last_direction * 100
				attack.rotation = last_direction.angle() + deg_to_rad(270)
				get_tree().current_scene.add_child(attack)
				return
			attack.global_position = global_position + direction * 100
			attack.rotation = direction.angle() + deg_to_rad(270)
			get_tree().current_scene.add_child(attack)

func handle_sprite(input_direction_vector: Vector2):
	$SideSprite.visible = false
	$FrontSprite.visible = false
	$BackSprite.visible = false

	if abs(input_direction_vector.y) > abs(input_direction_vector.x):
		if input_direction_vector.y > 0:
			$FrontSprite.visible = true
			last_facing_sprite = $FrontSprite
		else:
			$BackSprite.visible = true
			last_facing_sprite = $BackSprite
	else:
		if input_direction_vector.x != 0:
			$SideSprite.visible = true
			$SideSprite.flip_h = input_direction_vector.x > 0
			last_facing_sprite = $SideSprite
	if input_direction_vector.length() == 0:
		last_facing_sprite.visible = true

func _ready():
	add_to_group("players")
	$SideSprite.visible = true
	last_facing_sprite = $SideSprite
	hp_label = get_tree().current_scene.get_node("UI/HPLabel")
	update_hp_label()

func _physics_process(_delta: float) -> void:
	var input_aim_vector = Vector2(
		Input.get_action_strength("aim_right") - Input.get_action_strength("aim_left"),
		Input.get_action_strength("aim_down") - Input.get_action_strength("aim_up")
	)

	var input_direction_vector = Vector2(
		Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left"),
		Input.get_action_strength("ui_down") - Input.get_action_strength("ui_up")
	)

	handle_dash(input_direction_vector)
	handle_movement(input_direction_vector)
	slimy_boi_spawner()
	handle_attack(input_aim_vector, input_direction_vector)
	handle_sprite(input_direction_vector)
	move_and_slide()

func create_timer(
	wait: float,
	one_shot: bool = true,
	auto_start: bool = true,
	handler = null
) -> Timer:
	var timer = Timer.new()
	timer.wait_time = wait
	timer.one_shot = one_shot
	timer.autostart = auto_start
	add_child(timer)
	timer.connect("timeout", handler)
	return timer
