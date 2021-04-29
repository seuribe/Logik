using Godot;
using System;

public class NameEdit : LineEdit {


	public bool WorkMode { get; set; }

	public override void _Ready() {
		Connect("mouse_entered", this, "OnEnter");
		Connect("mouse_exited", this, "OnExit");
		Connect("focus_entered", this, "OnEnter");
		Connect("focus_exited", this, "OnExit");
	}

	private void OnEnter() {
		if (!WorkMode)
			Set("editable", true);
	}

	private void OnExit() {
		if (!HasFocus())
			Set("editable", false);
	}
}