using Godot;
using Logik.Core;

public class TableCellView : Control {

	private TabularCell tcell;
	private Control valueGrid;
	private Panel mainPanel;
	private GridCell viewTemplate;
	private GridCell[,] valueViews = new GridCell[1,1];
	private Rect2 viewRect;
	private readonly float HorizontalMargin = 16;
	private readonly float VerticalMargin = 16;

	public override void _Ready() {
		mainPanel = GetNode<Panel>("Main");
		valueGrid = mainPanel.GetNode<Control>("Grid");
		viewTemplate = GetNode<GridCell>("ValueTemplate");
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
		valueViews = new GridCell[tcell.Rows, tcell.Columns];
		mainPanel.RectSize = new Vector2(
			tcell.Columns * viewRect.Size.x + HorizontalMargin,
			tcell.Rows * viewRect.Size.y + VerticalMargin);

		for (int row = 0 ; row < tcell.Rows ; row++) {
			for (int column = 0 ; column < tcell.Columns ; column++) {
				AddGridCell(row, column);
			}
		}
	}

	private void AddGridCell(int row, int column) {
		var view = viewTemplate.Duplicate() as GridCell;
		valueViews[row, column] = view;
		valueGrid.AddChild(view);
		view.RectPosition = GetCellPosition(row, column);
		view.Text = tcell[row, column].ToString();
		view.Row = row;
		view.Column = column;
		view.ContentChanged += OnCellContentChanged;
		view.Visible = true;
	}
	
	private void RemoveGridCell(int row, int column) {
		var view = valueViews[row,column];
		if (view == null) 
			return;

		valueViews[row, column] = null;
		valueGrid.RemoveChild(view);
		view.QueueFree();
		view.ContentChanged -= OnCellContentChanged;
	}

	private void OnCellContentChanged(int row, int column) {
		tcell[row, column] = float.Parse(valueViews[row, column].Text);
	}

	private void OnTextEntered(string newText, int row, int column) {
		GD.Print($"Cell content modified: {newText} @ {row}x{column}");
	}

	private Vector2 GetCellPosition(int row, int column) => 
		new Vector2(HorizontalMargin/2 + column * viewRect.Size.x,
					VerticalMargin/2 + row * viewRect.Size.y);

	private void ClearGrid() {
		for (int row = 0 ; row < valueViews.GetLength(0) ; row++) {
			for (int column = 0 ; column < valueViews.GetLength(1) ; column++) {
				RemoveGridCell(row, column);
			}
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

