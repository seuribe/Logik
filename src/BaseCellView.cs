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

	private bool hover;
	public bool Hover {
		get => hover;
		private set {
			if (value != hover) {
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
			if (workMode == value)
				return;
			workMode = value;
			SwitchWorkMode();
			UpdateStyle();
        }
    }

	public override void _Ready() {
		var baseControls = GetNode<Control>("BaseControls");
		baseControls.GetNode<Panel>("DragArea").Connect("PositionChanged", this, nameof(OnPositionChanged));
		baseControls.GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(OnDeleteCellPressed));
	}

	public void SetCell(ICell cell) {
		if (Cell != null)
			StopObserving(Cell);

		Cell = cell;
		StartObserving(Cell);
		UpdateView();
	}

	protected void StartObserving(ICell cell) {
		cell.ValueChanged += CellValueChanged;
		cell.ErrorStateChanged += CellErrorStateChanged;
	}

	protected void StopObserving(ICell cell) {
		cell.ValueChanged -= CellValueChanged;
		cell.ErrorStateChanged -= CellErrorStateChanged;
	}

	private void CellErrorStateChanged(ICell cell) {
		UpdateView();
	}

	private void CellValueChanged(ICell cell) {
		UpdateView();
	}

	private void OnDeleteCellPressed() {
		((ConfirmationDialog)GetNode("DeleteCellDialog")).PopupCentered();
	}

	private void DeleteCellConfirmed() {
		DeleteCell?.Invoke(Cell);
	}

	public void Delete() {
		StopObserving(Cell);
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
