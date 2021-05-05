using Godot;
using Logik.Core;

public class CellViewState {
	public Vector2 position;
	public bool inputOnly = false;

	public CellViewState(Vector2 position = new Vector2()) {
		this.position = position;
	}
	public CellViewState(CellView cellView) {
		position = cellView.RectPosition;
		inputOnly = cellView.InputOnly;
	}
}

public abstract class BaseCellView : Control {
	public abstract event CellEvent DeleteCell;
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

public class CellView : BaseCellView {

	public override event CellEvent DeleteCell;

	private Label valueLabel;
	private Label errorLabel;
	private Label formulaLabel;
	private NameEdit nameEdit;
	private NameEdit valueEdit;
	private LineEdit formulaText;
	private Panel mainControls;
	private Control extraControls;

	protected new NumericCell Cell {
		get => base.Cell as NumericCell;
		set => base.Cell = value;
	}

	private bool workMode = false;
	public bool WorkMode {
		get => workMode;
		set {
			workMode = value;
			extraControls.Visible = workMode;
			nameEdit.WorkMode = workMode;
			UpdateStyle();
		}
	}

	private bool inputOnly = false;
	public bool InputOnly {
		get => inputOnly;
		set {
			inputOnly = value;
			if (inputOnly) {
				valueEdit.Show();
				valueLabel.Hide();
				formulaText.Hide();
				formulaLabel.Hide();
			} else {
				valueEdit.Hide();
				valueLabel.Show();
				formulaText.Show();
				formulaLabel.Show();
			}
		}
	}

	public override void _Ready() {
		mainControls = GetNode<Panel>("Main");
		nameEdit = mainControls.GetNode<NameEdit>("NameEdit");
		valueLabel = mainControls.GetNode<Label>("ValueLabel");
		valueEdit = mainControls.GetNode<NameEdit>("ValueEdit");
		errorLabel = mainControls.GetNode<Label>("ErrorLabel");

		extraControls = GetNode<Control>("ExtraControls");
		formulaText = extraControls.GetNode<LineEdit>("FormulaText");
		formulaLabel = extraControls.GetNode<Label>("FormulaLabel");

		var dragAreaPanel = extraControls.GetNode<Panel>("DragArea");
		dragAreaPanel.Connect("PositionChanged", this, "OnPositionChanged");

		extraControls.GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(OnDeleteCellPressed));

		nameEdit.TextChanged += OnNameChanged;
	}

	protected override void UpdateValuesFromCell() {
		valueLabel.Text = Cell.Error ? " - " : Cell.Value.ToString();
		valueEdit.Text = valueLabel.Text;
		nameEdit.Text = Cell.Name;
		if (!formulaText.HasFocus())
			formulaText.Text = Cell.Formula;
		errorLabel.Text = Cell.ErrorMessage;
	}

	protected override void UpdateStyle() {
		mainControls.Set("custom_styles/panel", (Hover && !WorkMode) ? StyleHover : (Cell.Error ? StyleError : StyleNormal));
		if (!WorkMode && (Hover || formulaText.HasFocus()))
			extraControls.Show();
		else
			extraControls.Hide();
	}

	public void OnFormulaChanged(string newFormula) {
		Cell.Formula = string.IsNullOrEmpty(newFormula) ? "0" : newFormula;
		UpdateView();
	}

	public void OnFormulaFocusExited() {
		UpdateStyle();
	}

	private void OnNameChanged(string newName) {
		if (newName != Cell.Name) {
			Cell.TryNameChange(newName);
			nameEdit.Set("editable", false);
			UpdateView();
		}
	}

	private void OnDeleteCellPressed() {
		((ConfirmationDialog)GetNode("DeleteCellDialog")).PopupCentered();
	}

	private void DeleteCellConfirmed() {
		DeleteCell?.Invoke(Cell);
	}

	private void OnValueChanged(string newValue) {
		if (float.TryParse(newValue, out float value)) {
			OnFormulaChanged(newValue);
			Cell.ClearError();
		} else {
			Cell.SetError("Invalid value");
		}
		UpdateStyle();
	}

	private void OnValueChanged() {
		OnValueChanged(valueEdit.Text);
	}

	private void OnInputOnlyToggle(bool pressed) {
		InputOnly = pressed;
	}
}
