using Godot;
using Logik.Core;


public class CellView : Control {

	public event CellEvent DeleteCell;
	public event CellEvent PositionChanged;

	private Label valueLabel;
	private Label errorLabel;
	private Label formulaLabel;
	private LineEdit nameText;
	private LineEdit formulaText;
	private Panel dragAreaPanel;
	private Panel mainBG;
	private NumericCell cell;
	private Control hideArea;

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

	public Vector2 ConnectorStart { get => RectPosition + formulaText.RectPosition + (formulaText.RectSize/2); }
	public Vector2 ConnectorEnd { get => RectPosition + valueLabel.RectPosition + (valueLabel.RectSize/2); }

	private static readonly StyleBoxFlat StyleError = GD.Load<StyleBoxFlat>("res://styles/cell_error.tres");
	private static readonly StyleBoxFlat StyleNormal = GD.Load<StyleBoxFlat>("res://styles/cell_normal.tres");
	private static readonly StyleBoxFlat StyleHover = GD.Load<StyleBoxFlat>("res://styles/cell_hover.tres");

	private Vector2 GetConnectorPosition(string connector) {
		return RectPosition + ((Control)GetNode("Connector"+connector)).RectPosition;
	}

	public override void _Ready() {
		mainBG = (Panel)GetNode("Panel/MainBG");
		valueLabel = (Label)GetNode("Panel/ValueLabel");
		errorLabel = (Label)GetNode("Panel/ErrorLabel");
		nameText = (LineEdit)GetNode("Panel/NameText");
		
		hideArea = (Control)GetNode("Panel/HideArea");
		formulaText = (LineEdit)GetNode("Panel/HideArea/FormulaText");
		formulaLabel = (Label)GetNode("Panel/HideArea/FormulaLabel");
		dragAreaPanel = (Panel)GetNode("Panel/HideArea/DragArea");

		nameText.Connect("mouse_entered", this, "OnMouseEnterName");
		nameText.Connect("mouse_exited", this, "OnMouseExitName");
		nameText.Connect("focus_entered", this, "OnMouseEnterName");
		nameText.Connect("focus_exited", this, "OnMouseExitName");
	}
	private void OnMouseEnterName() {
		nameText.Set("editable", true);
	}

	private void OnMouseExitName() {
		if (!nameText.HasFocus())
			nameText.Set("editable", false);
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
		nameText.Text = cell.Name;
		if (!formulaText.HasFocus())
			formulaText.Text = cell.Formula;
		errorLabel.Text = cell.ErrorMessage;
		UpdateStyle();
		Update();
	}

	private void UpdateStyle() {
		mainBG.Set("custom_styles/panel", Hover ? StyleHover : (cell.Error ? StyleError : StyleNormal));
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
		OnNameChanged(nameText.Text);
	}

	public void OnNameChanged(string newName) {
		if (newName != cell.Name) {
			cell.TryNameChange(newName);
			nameText.Set("editable", false);
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
		hideArea.Hide();
	}

	private void ShowExtraControls() {
		hideArea.Show();
	}

}
