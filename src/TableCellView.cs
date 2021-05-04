using Godot;
using System;
using Logik.Core;

public class TableCellView : Control {

	private TabularCell tcell;
	private Control valueGrid;
	private Panel mainPanel;
	private LineEdit viewTemplate;
	private LineEdit[,] valueViews = new LineEdit[1,1];
	private Rect2 viewRect;
	private readonly float HorizontalMargin = 16;
	private readonly float VerticalMargin = 16;

	public override void _Ready() {
		mainPanel = GetNode<Panel>("Main");
		valueGrid = mainPanel.GetNode<Control>("Grid");
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
		mainPanel.RectSize = new Vector2(
			tcell.Columns * viewRect.Size.x + HorizontalMargin,
			tcell.Rows * viewRect.Size.y + VerticalMargin);
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
		new Vector2(HorizontalMargin/2 + column * viewRect.Size.x,
					VerticalMargin/2 + row * viewRect.Size.y);


	private void ClearGrid() {
		foreach (var view in valueViews) {
			if (view != null)
				valueGrid.RemoveChild(view);
		}
	}
	
	private void OnAddColumn() {
		AddRowsColumns(0, 1);
	}

	private void OnAddRow() {
		AddRowsColumns(1, 0);
	}

	private void OnRemoveRow() {
		AddRowsColumns(-1, 0);
	}

	private void OnRemoveColumn() {
		AddRowsColumns(0, -1);
	}

	private void AddRowsColumns(int dRows, int dColumns) {
		tcell.Resize(tcell.Rows + dRows, tcell.Columns + dColumns);
		ClearGrid();
		CreateGrid();
	}

}

