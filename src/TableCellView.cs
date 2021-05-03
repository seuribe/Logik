using Godot;
using System;
using Logik.Core;

public class TableCellView : Control {

	private TabularCell tcell;
	private Control valueGrid;
	private LineEdit viewTemplate;
	private LineEdit[,] valueViews = new LineEdit[1,1];
	private Rect2 viewRect;

	public override void _Ready() {
		valueGrid = GetNode<Control>("Main/Grid");
		viewTemplate = GetNode<LineEdit>("ValueTemplate");
		viewRect = viewTemplate.GetRect();

		var dragAreaPanel = GetNode<Panel>("DragArea");
		dragAreaPanel.Connect("PositionChanged", this, "OnPositionChanged");
	}
	
	private void OnPositionChanged(Vector2 newPosition) {
		RectPosition = newPosition;
	}

	public void SetCell(TabularCell tcell) {
		this.tcell = tcell;
		ClearGrid();
		CreateGrid();
	}

	private void CreateGrid() {
		valueViews = new LineEdit[tcell.Rows, tcell.Columns];
		for (int row = 0 ; row < tcell.Rows ; row++) {
			for (int column = 0 ; column < tcell.Columns ; column++) {
				var view = viewTemplate.Duplicate() as LineEdit;
				valueGrid.AddChild(view);
				view.RectPosition = GetCellPosition(row, column);
				view.Text = tcell[row, column].ToString();
				view.Visible = true;
			}
		}
	}

	private Vector2 GetCellPosition(int row, int column) => 
		new Vector2(viewRect.Position.x + column * viewRect.Size.x, row * viewRect.Size.y);


	private void ClearGrid() {
		foreach (var view in valueViews) {
			if (view != null)
				valueGrid.RemoveChild(view);
		}
	}
}
