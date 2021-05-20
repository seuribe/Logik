using Godot;
using Logik.Core;

/// <summary>
/// Helper class to transmit visual & state information about a cell.
/// </summary>
public struct CellViewState {
	public readonly Vector2 position;
	public readonly bool inputOnly;

	public CellViewState(Vector2 position = new Vector2(), bool inputOnly = false) {
		this.position = position;
		this.inputOnly = inputOnly;
	}
	public CellViewState(BaseCellView cellView) {
		position = cellView.RectPosition;
		inputOnly = cellView.InputOnly;
	}
}

public class CellView : BaseCellView {

	private Label valueLabel;
	private Label errorLabel;
	private Label formulaLabel;
	private NameEdit nameEdit;
	private NameEdit valueEdit;
	private LineEdit formulaText;
	private Panel mainControls;
	private Control extraControls;
	private Control baseControls;
	private Button inputToggle;

	protected override string DragAreaNodePath { get => "BaseControls/DragArea"; }
	protected override string DeleteButtonNodePath { get => "BaseControls/DeleteButton"; }
	protected override string DeleteDialogNodePath { get => "DeleteCellDialog"; }


	protected new FormulaCell Cell {
		get => base.Cell as FormulaCell;
		set => base.Cell = value;
	}

	protected override void SwitchWorkMode() {
		extraControls.Visible = WorkMode;
		nameEdit.WorkMode = WorkMode;
	}

	private bool inputOnly = false;
	public override bool InputOnly {
		get => inputOnly;
		set {
			inputOnly = value;

			valueEdit.Visible = inputOnly;
			valueLabel.Visible = !inputOnly;
			formulaText.Visible = !inputOnly;
			formulaLabel.Visible = !inputOnly;

			// Ensure that the value of the button (UI) is correct after loading the model from storage
			inputToggle.Pressed = inputOnly; 
		}
	}

	public override void _Ready() {
		base._Ready();

		mainControls = GetNode<Panel>("Main");
		nameEdit = mainControls.GetNode<NameEdit>("NameEdit");
		valueLabel = mainControls.GetNode<Label>("ValueLabel");
		valueEdit = mainControls.GetNode<NameEdit>("ValueEdit");
		errorLabel = mainControls.GetNode<Label>("ErrorLabel");
		baseControls = GetNode<Control>("BaseControls");

		extraControls = GetNode<Control>("ExtraControls");
		formulaText = extraControls.GetNode<LineEdit>("FormulaText");
		formulaLabel = extraControls.GetNode<Label>("FormulaLabel");
		inputToggle = extraControls.GetNode<Button>("InputToggle");

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
		extraControls.Visible = !WorkMode && (Hover || formulaText.HasFocus());
		baseControls.Visible = !WorkMode && Hover;
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

	private void OnValueChanged(string newValue) {
		Cell.ClearError();
		OnFormulaChanged(newValue);
		UpdateStyle();
	}

	private void OnValueChanged() {
		OnValueChanged(valueEdit.Text);
	}

	private void OnInputOnlyToggle(bool pressed) {
		InputOnly = pressed;
	}
}
