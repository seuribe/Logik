using Godot;
using Logik.View;

public class DragControl : Panel {


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
			if (ModelView.SnapToGrid && LayoutGrid.TryFindSnapPoint(newPosition, out Vector2 snapPoint)) {
				newPosition = snapPoint;
			}
			EmitSignal(nameof(PositionChanged), newPosition);
		} else if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.Pressed) {
			dragging = false;
		}
	}

}
