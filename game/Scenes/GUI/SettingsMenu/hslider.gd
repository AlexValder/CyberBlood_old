extends HBoxContainer
class_name HSliderWithLabel

onready var _slider := $hslider as Slider
onready var _text := $text as Label

func _ready() -> void:
    _slider.connect("value_changed", self, "_update_text")


func _update_text(value: float) -> void:
    _text.text = "%.1f" % stepify(value, 0.1)
