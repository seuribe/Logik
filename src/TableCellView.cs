using Godot;
using Logik.Core;

public class TableCellView : BaseCellView {

	private Panel valueGrid;
	private Panel mainPanel;
	private Label errorLabel;
	private NameEdit nameEdit;
	private GridCell viewTemplate;
	private GridCell[,] valueViews = new GridCell[1,1];
	private Control sizeControls;
	private Control baseControls;
	private Rect2 viewRect;
	private readonly float HorizontalMargin = 16;
	private readonly float VerticalMargin = 16;

	protected override string DragAreaNodePath { get => "BaseControls/DragArea"; }
	protected override string DeleteButtonNodePath { get => "BaseControls/DeleteButton"; }
	protected override string DeleteDialogNodePath { get => "DeleteCellDialog"; }

	protected new TabularCell Cell {
		get => base.Cell as TabularCell;
		set => base.Cell = value;
	}

	public override void _Ready() {
		base._Ready();

		mainPanel = GetNode<Panel>("Main");
		valueGrid = mainPanel.GetNode<Panel>("Grid");
		viewTemplate = GetNode<GridCell>("ValueTemplate");
		viewRect = viewTemplate.GetRect();
		errorLabel = mainPanel.GetNode<Label>("ErrorLabel");
		sizeControls = mainPanel.GetNode<Control>("SizeControls");
		baseControls = GetNode<Control>("BaseControls");

		nameEdit = GetNode<NameEdit>("Main/NameEdit");
		nameEdit.TextChanged += OnNameChanged;
	}
	
	protected override void SwitchWorkMode() {
		UpdateStyle();
    }

	private void OnNameChanged(string newName) {
		if (newName != Cell.Name) {
			if (!Cell.TryNameChange(newName))
				nameEdit.Text = Cell.Name;

			nameEdit.Set("editable", false);
		}
	}

	public void SetCell(TabularCell tcell) {
		Cell = tcell;
		ClearGrid();
		CreateGrid();
		UpdateValuesFromCell();
	}

	private void CreateGrid() {
		valueViews = new GridCell[Cell.Rows, Cell.Columns];
		valueGrid.RectSize = new Vector2(
			Cell.Columns * viewRect.Size.x + HorizontalMargin,
			Cell.Rows * viewRect.Size.y + VerticalMargin);

		for (int row = 0 ; row < Cell.Rows ; row++) {
			for (int column = 0 ; column < Cell.Columns ; column++) {
				AddGridCell(row, column);
			}
		}
	}

	private void AddGridCell(int row, int column) {
		var view = viewTemplate.Duplicate() as GridCell;
		valueViews[row, column] = view;
		valueGrid.AddChild(view);
		view.RectPosition = GetCellPosition(row, column);
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
		var strValue = valueViews[row, column].Text;
		if (float.TryParse(strValue, out float value)) {
			Cell[row, column] = value;
		} else {
			Cell.SetError($"Invalid value {strValue} at {row},{column}");
		}
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

	protected override void UpdateValuesFromCell() {
		nameEdit.Text = Cell.Name;
		for (int row = 0 ; row < Cell.Rows ; row++) {
			for (int column = 0 ; column < Cell.Columns ; column++) {
				valueViews[row, column].Text = Cell[row, column].ToString();
			}
		}
		errorLabel.Text = Cell.ErrorMessage;
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
		Cell.Resize(Cell.Rows + dRows, Cell.Columns + dColumns);
		ClearGrid();
		CreateGrid();
		UpdateValuesFromCell();
	}

	protected override void UpdateStyle() {
		baseControls.Visible = Hover && !WorkMode;
		sizeControls.Visible = Hover && !WorkMode;
		nameEdit.WorkMode = WorkMode;
	}
}

