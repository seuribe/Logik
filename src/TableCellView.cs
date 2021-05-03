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
		valueViews[0,0] = valueGrid.GetNode<LineEdit>("ValueEdit");
		viewTemplate = GetNode<LineEdit>("ValueTemplate");
		viewRect = viewTemplate.GetRect();
	}

	public void SetCell(TabularCell tcell) {
		this.tcell = tcell;
		CreateGrid();
	}

	private void CreateGrid() {
		foreach (var view in valueViews) {
			valueGrid.RemoveChild(view);
		}
		valueViews = new LineEdit[tcell.Rows, tcell.Columns];
		for (int row = 0 ; row < tcell.Rows ; row++) {
			for (int column = 0 ; column < tcell.Columns ; column++) {
				var view = viewTemplate.Duplicate() as LineEdit;
				view.RectPosition = new Vector2(column * viewRect.Size.x, row * viewRect.Size.y);
				view.Text = tcell[row, column].ToString();
			}
		}
	}

}
