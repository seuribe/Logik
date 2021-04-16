using Godot;
using Logik.Core;


public class CellView : Control {

	public event CellEvent DeleteCell;
	public event CellEvent PositionChanged;

	private Label valueLabel;
	private Label errorLabel;
	private NameEdit nameEdit;
	private LineEdit formulaText;
	private Panel dragAreaPanel;
	private Panel mainControls;
	private NumericCell cell;
	private Control extraControls;

	private bool dragging = false;
	private Vector2 dragOffset;
	private bool hover;
	public bool Hover {
		get => hover;
		private set {
			if (value != hover) {
				hover = value;
				UpdateStyle();
				(GetParent() as Control).Update();
			}
		}
	}

	public Vector2 ConnectorLeft { get => GetConnectorPosition("Left"); }
	public Vector2 ConnectorTop { get => GetConnectorPosition("Top"); }
	public Vector2 ConnectorRight { get => GetConnectorPosition("Right"); }
	public Vector2 ConnectorBottom { get => GetConnectorPosition("Bottom"); }

	private static readonly StyleBoxFlat StyleError = GD.Load<StyleBoxFlat>("res://styles/cell_error.tres");
	private static readonly StyleBoxFlat StyleNormal = GD.Load<StyleBoxFlat>("res://styles/cell_normal.tres");
	private static readonly StyleBoxFlat StyleHover = GD.Load<StyleBoxFlat>("res://styles/cell_hover.tres");

	private Vector2 GetConnectorPosition(string connector) {
		return RectPosition + ((Control)GetNode("Connectors/" + connector)).RectPosition;
	}

	public override void _Ready() {
		mainControls = GetNode<Panel>("Main");
		nameEdit = mainControls.GetNode<NameEdit>("NameEdit");
		valueLabel = mainControls.GetNode<Label>("ValueLabel");
		errorLabel = mainControls.GetNode<Label>("ErrorLabel");
		
		extraControls = GetNode<Control>("ExtraControls");
		formulaText = extraControls.GetNode<LineEdit>("FormulaText");
		dragAreaPanel = extraControls.GetNode<Panel>("DragArea");
	}

	public void SetCell(NumericCell cell) {
		if (this.cell != null)
			StopObserving(this.cell);

		this.cell = cell;
		StartObserving(cell);
		UpdateView();
	}

	private void StartObserving(NumericCell cell) {
		cell.ValueChanged += CellValueChanged;
		cell.ErrorStateChanged += CellErrorStateChanged;
	}

	private void StopObserving(NumericCell cell) {
		cell.ValueChanged -= CellValueChanged;
		cell.ErrorStateChanged -= CellErrorStateChanged;
	}

	private void CellErrorStateChanged(NumericCell cell) {
		UpdateView();
	}

	private void CellValueChanged(NumericCell cell) {
		UpdateView();
	}

	private void UpdateView() {
		valueLabel.Text = cell.Error ? " - " : cell.Value.ToString();
		nameEdit.Text = cell.Name;
		if (!formulaText.HasFocus())
			formulaText.Text = cell.Formula;
		errorLabel.Text = cell.ErrorMessage;
		UpdateStyle();
		Update();
	}

	private void UpdateStyle() {
		mainControls.Set("custom_styles/panel", Hover ? StyleHover : (cell.Error ? StyleError : StyleNormal));
		if (Hover || formulaText.HasFocus())
			ShowExtraControls();
		else
			HideExtraControls();
	}

	public void OnFormulaChanged(string newFormula) {
		cell.Formula = string.IsNullOrEmpty(newFormula) ? "0" : newFormula;
		UpdateView();
	}

	public void OnFormulaFocusExited() {
		UpdateStyle();
	}

	public void OnNameChanged() {
		OnNameChanged(nameEdit.Text);
	}

	public void OnNameChanged(string newName) {
		if (newName != cell.Name) {
			cell.TryNameChange(newName);
			nameEdit.Set("editable", false);
			UpdateView();
		}
	}

	private void OnDeleteCellPressed() {
		((ConfirmationDialog)GetNode("DeleteCellDialog")).PopupCentered();
	}

	private void DeleteCellConfirmed() {
		DeleteCell?.Invoke(cell);
	}

	public void Delete() {
		StopObserving(cell);
	}

	public override void _Input(InputEvent @event) {
		if (dragging) {
			UpdateDrag(@event);
		} else {
			CheckForStartDrag(@event);
		}
		CheckForHover(@event);
	}

	private void CheckForHover(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion) {
			Hover = GetRect().HasPoint(eventMouseMotion.Position);
		}
	}

	private void CheckForStartDrag(InputEvent @event) {
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
			Rect2 dragArea = new Rect2(RectPosition + dragAreaPanel.RectPosition, dragAreaPanel.RectSize);
			if (dragArea.HasPoint(eventMouseButton.Position)) {
				dragging = true;
				dragOffset = eventMouseButton.Position - RectPosition;
			}
		}
	}

	private void UpdateDrag(InputEvent @event) {
		if (@event is InputEventMouseMotion eventMouseMotion) {
			RectPosition = eventMouseMotion.Position - dragOffset;
			PositionChanged?.Invoke(cell);
		} else if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.Pressed) {
			dragging = false;
		}
	}

	private void HideExtraControls() {
		extraControls.Hide();
	}

	private void ShowExtraControls() {
		extraControls.Show();
	}

}
