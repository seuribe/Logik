using Godot;
using Logik.Core;

public abstract class BaseCellView : Control {
	public event CellEvent DeleteCell;
	public event CellEvent PositionChanged;

	public Vector2 ConnectorLeft { get => GetConnectorPosition("Left"); }
	public Vector2 ConnectorTop { get => GetConnectorPosition("Top"); }
	public Vector2 ConnectorRight { get => GetConnectorPosition("Right"); }
	public Vector2 ConnectorBottom { get => GetConnectorPosition("Bottom"); }

	protected static readonly StyleBoxFlat StyleError = GD.Load<StyleBoxFlat>("res://styles/cell_error.tres");
	protected static readonly StyleBoxFlat StyleNormal = GD.Load<StyleBoxFlat>("res://styles/cell_normal.tres");
	protected static readonly StyleBoxFlat StyleHover = GD.Load<StyleBoxFlat>("res://styles/cell_hover.tres");

	private Vector2 GetConnectorPosition(string connector) {
		return RectPosition + ((Control)GetNode("Connectors/" + connector)).RectPosition;
	}
	
	protected abstract void UpdateStyle();
	protected abstract void UpdateValuesFromCell();
	protected abstract void SwitchWorkMode();

	protected ICell Cell { get; set; }

	protected abstract string DragAreaNodePath { get; }
	protected abstract string DeleteButtonNodePath { get; }
	protected abstract string DeleteDialogNodePath { get; }

	public virtual bool InputOnly { get; set; } = false;

	private bool hover;
	public bool Hover {
		get => hover;
		private set {
			if (hover != value) {
				hover = value;
				UpdateStyle();
				(GetParent() as Control).Update(); // force redraw of connectors
			}
		}
	}
	
	private bool workMode = false;
	public bool WorkMode {
		get => workMode;
		set {
			if (workMode != value) {
				workMode = value;
				SwitchWorkMode();
				UpdateStyle();
            }
        }
    }

	public override void _Ready() {
		GetNode<Panel>(DragAreaNodePath).Connect("PositionChanged", this, nameof(OnPositionChanged));
		GetNode<Button>(DeleteButtonNodePath).Connect("pressed", this, nameof(OnDeleteCellPressed));
	}

	public void SetCell(ICell cell) {
		StopObservingCell();
		Cell = cell;
		StartObservingCell();
		UpdateView();
	}

	protected void StartObservingCell() {
		Cell.ValueChanged += CellValueChanged;
		Cell.ErrorStateChanged += CellErrorStateChanged;
	}

	protected void StopObservingCell() {
		if (Cell == null)
			return;
		Cell.ValueChanged -= CellValueChanged;
		Cell.ErrorStateChanged -= CellErrorStateChanged;
	}

	private void CellErrorStateChanged(ICell cell) {
		UpdateView();
	}

	private void CellValueChanged(ICell cell) {
		UpdateView();
	}

	private void OnDeleteCellPressed() {
		var confirmDialog = ((ConfirmationDialog)GetNode(DeleteDialogNodePath));
		confirmDialog.Connect("confirmed", this, nameof(DeleteCellConfirmed));
		confirmDialog.PopupCentered();
	}

	private void DeleteCellConfirmed() {
		DeleteCell?.Invoke(Cell);
	}

	public void Delete() {
		StopObservingCell();
	}

	protected void UpdateView() {
		UpdateValuesFromCell();
		UpdateStyle();
		Update();
	}
	
	private void OnPositionChanged(Vector2 newPosition) {
		RectPosition = newPosition;
		PositionChanged?.Invoke(Cell);
	}
	
	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion) {
			Hover = GetRect().HasPoint(eventMouseMotion.Position);
		}
	}
}
