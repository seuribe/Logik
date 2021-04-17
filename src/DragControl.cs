using Godot;
using System;

public class DragControl : Panel {

	const int GridSize = 100;
	const int SnapDistance = 20;

	[Signal]
	public delegate void PositionChanged(Vector2 position);

	private bool dragging = false;
	private Vector2 dragStart;

	public override void _Input(InputEvent @event) {
		if (dragging) {
			UpdateDrag(@event);
		} else {
			CheckForStartDrag(@event);
		}
	}

	private void CheckForStartDrag(InputEvent @event) {
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
			if (GetGlobalRect().HasPoint(eventMouseButton.Position)) {
				dragging = true;
				dragStart = eventMouseButton.Position - RectGlobalPosition;
			}
		}
	}

	private void UpdateDrag(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion) {
			var newPosition = eventMouseMotion.Position - dragStart;
			if (TryFindSnapPoint(newPosition, out Vector2 snapPoint)) {
				newPosition = snapPoint;
			}
			EmitSignal(nameof(PositionChanged), newPosition);
		} else if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.Pressed) {
			dragging = false;
		}
	}

	private bool TryFindSnapPoint(Vector2 position, out Vector2 snapPoint) {


		var rx = (position.x + SnapDistance) % GridSize;
		var ry = (position.y + SnapDistance) % GridSize;

		if (rx <= (2 * SnapDistance) && ry <= (2 * SnapDistance) ) {
			var dx = (position.x + SnapDistance) / GridSize;
			var dy = (position.y + SnapDistance) / GridSize;
			var snapX = ((int)dx) * GridSize;
			var snapY = ((int)dy) * GridSize;
			snapPoint = new Vector2(snapX, snapY);
			return true;
		}
		snapPoint = position;
		return false;
/*

		var snap = (Math.Abs(position.x % GridSize) < SnapDistance &&
					Math.Abs(position.y % GridSize) < SnapDistance);
		snapPoint = (snap) ? 
		var diff = position - new Vector2(100, 100);
		if (Math.Abs(diff.x) < 10 && Math.Abs(diff.y) < 10) {
			snapPoint = new Vector2(100, 100);
			return true;
		} else {
			snapPoint = position;
		}
		return false;
*/
	}

}
