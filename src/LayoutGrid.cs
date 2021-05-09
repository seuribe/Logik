using Godot;

namespace Logik.View {
	public class LayoutGrid {
		public static Vector2 GridSize = new Vector2(100, 100);
		public static Vector2 SnapDistance = new Vector2(20, 20);
		
		public static bool TryFindSnapPoint(Vector2 position, out Vector2 snapPoint) {
			var r = (position + SnapDistance) % GridSize;

			if (r < (SnapDistance * 2)) {
				snapPoint = position.Snapped(GridSize);
				return true;
			}
			snapPoint = position;
			return false;
		}

	}

}
