extends CharacterBody2D


const SPEED = 200
const DASH_SPEED = 1000
const LightMeleeFire = preload("res://attacks/light/light_melee_fire.tscn")
const SlimyBoi = preload("res://enemies/slimy_boi.tscn")

var creating_slime := false
var is_dashing = false
var can_dash := true
var dash_direction = Vector2.ZERO
var last_direction := Vector2.RIGHT
var last_facing_sprite: Node = null
var active_slimes := []
var health := 5



func take_damage(damage: int):
	health -= damage
	print("player took damage. health is now " + str(health))
	if health <= 0:
		queue_free()


func create_slimy_boi():
	creating_slime = false
	print("creating slimy boi")
	var slime = SlimyBoi.instantiate()
	slime.global_position = get_viewport_rect().size / 2
	slime.player_ref = self
	get_tree().current_scene.add_child(slime)
	active_slimes.append(slime)
	slime.connect("tree_exited", Callable(self, "_on_slime_exited").bind(slime))


func _on_slime_exited(slime):
	active_slimes.erase(slime)

func _ready():
	add_to_group("players")
	$SideSprite.visible = true
	last_facing_sprite = $SideSprite
	
func _physics_process(_delta: float) -> void:


	var input_vector = Vector2(
		Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left"),
		Input.get_action_strength("ui_down") - Input.get_action_strength("ui_up")
		)


	if active_slimes.size() == 0 and creating_slime == false:
		create_timer(2.0, true, true, func(): create_slimy_boi())
		creating_slime = true
		

		
	
	if Input.is_action_just_pressed("dash") and can_dash and input_vector.length() > 0:
		is_dashing = true
		create_timer(0.2, true, true, func(): is_dashing = false)
		can_dash = false
		create_timer(1.0, true, true, func(): can_dash = true)
		dash_direction = input_vector.normalized()


	if is_dashing:
		velocity = dash_direction * DASH_SPEED
	else:
		if input_vector.length() > 1:
			input_vector = input_vector.normalized()
		velocity = input_vector * SPEED


	#Creates and set direction and last_direction
	var direction = input_vector.normalized()
	if input_vector.length() > 0:
		last_direction = direction


	if Input.is_action_just_pressed("attack_light"):
		var attack = LightMeleeFire.instantiate()
		if input_vector.length() == 0:
			attack.global_position = global_position + last_direction * 100
			attack.rotation = last_direction.angle() + deg_to_rad(270)
			get_tree().current_scene.add_child(attack)
			return
		attack.global_position = global_position + direction * 100
		attack.rotation = input_vector.angle() + deg_to_rad(270)
		get_tree().current_scene.add_child(attack)



	# Hide all sprites first
	$SideSprite.visible = false
	$FrontSprite.visible = false
	$BackSprite.visible = false

	#Handles Sprite facing direction
	if abs(input_vector.y) > abs(input_vector.x):
		if input_vector.y > 0:
			$FrontSprite.visible = true
			last_facing_sprite = $FrontSprite
		else:
			$BackSprite.visible = true
			last_facing_sprite = $BackSprite
	else:
		if input_vector.x != 0:
			$SideSprite.visible = true
			$SideSprite.flip_h = input_vector.x > 0
			last_facing_sprite = $SideSprite
	# If not moving, show last sprite
	if input_vector.length() == 0:
		last_facing_sprite.visible = true
		
		

	
	
	move_and_slide()





func create_timer(
	wait: float,
	one_shot: bool = true,
	auto_start: bool = true,
	handler = null
) -> Timer:
	# print("creating timer")
	var timer = Timer.new()
	timer.wait_time = wait
	timer.one_shot = one_shot
	timer.autostart = auto_start
	# print("starting timer")
	add_child(timer)
	timer.connect("timeout", handler)
	return timer
	
