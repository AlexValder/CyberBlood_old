extends Control
class_name PauseMenu

onready var _bg := $bg as ColorRect
onready var _vbox := $vbox as VBoxContainer
onready var _first_button := $vbox/resume as Control

signal pause_toggled(new_state)


func _unhandled_input(event: InputEvent) -> void:
    if event.is_action_released("pause"):
        _toggle_pause(!visible)
    elif event.is_action_released("ui_cancel") && visible:
        _toggle_pause(false)


func _toggle_pause(pause: bool) -> void:
    GameManager.TogglePause(!pause)
    visible = pause
    if pause:
        Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
        _first_button.grab_focus()
    else:
        Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)

    emit_signal("pause_toggled", pause)


func _on_resume_button_up() -> void:
    _toggle_pause(false)


func _on_restart_button_up() -> void:
    # TODO: not everything?
    _toggle_pause(false)
    GameManager.Restart()


func _on_exit_button_up() -> void:
    # TODO: exit to main menu
    _toggle_pause(false)
    GameManager.QuitToMenu()
