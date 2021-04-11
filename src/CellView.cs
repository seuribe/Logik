using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class CellView : Control {

	public event CellEvent DeleteCell;

	private Label valueLabel;
	private Label errorLabel;
	private LineEdit nameText;
	private LineEdit formulaText;
	private Panel dragAreaPanel;
	private Panel mainPanel;
	private Cell cell;

	private bool dragging = false;
	private Vector2 dragOffset;

	public Vector2 ConnectorLeft { get => GetConnectorPosition("Left"); }
	public Vector2 ConnectorTop { get => GetConnectorPosition("Top"); }
	public Vector2 ConnectorRight { get => GetConnectorPosition("Right"); }
	public Vector2 ConnectorBottom { get => GetConnectorPosition("Bottom"); }

	public Vector2 ConnectorStart { get => RectPosition + formulaText.RectPosition + (formulaText.RectSize/2); }
	public Vector2 ConnectorEnd { get => RectPosition + valueLabel.RectPosition + (valueLabel.RectSize/2); }

	private static readonly StyleBoxFlat StyleError = GD.Load<StyleBoxFlat>("res://styles/cell_error.tres");
	private static readonly StyleBoxFlat StyleNormal = GD.Load<StyleBoxFlat>("res://styles/cell_normal.tres");

	private Vector2 GetConnectorPosition(string connector) {
		return RectPosition + ((Control)GetNode("Connector"+connector)).RectPosition;
	}

	public override void _Ready() {
		valueLabel = (Label)GetNode("Panel/ValueLabel");
		errorLabel = (Label)GetNode("Panel/ErrorLabel");
		nameText = (LineEdit)GetNode("Panel/NameText");
		formulaText = (LineEdit)GetNode("Panel/FormulaText");
		dragAreaPanel = (Panel)GetNode("Panel/DragArea");
		mainPanel = (Panel)GetNode("Panel");
	}

	public void SetCell(Cell cell) {
		if (this.cell != null)
			this.cell.ValueChanged -= CellValueChanged;
		this.cell = cell;

		cell.ValueChanged += CellValueChanged;
		UpdateView();
	}

	private void CellValueChanged(Cell cell) {
		UpdateView();
	}

	private void UpdateView() {
		valueLabel.Text = cell.Value;
		nameText.Text = cell.Name;
		if (!formulaText.HasFocus())
			formulaText.Text = cell.Formula;
		errorLabel.Text = cell.ErrorMessage;
		mainPanel.Set("custom_styles/panel", (cell.Error) ? StyleError : StyleNormal);
		Update();
	}

	public void OnFormulaChanged(string newFormula) {
		cell.Formula = string.IsNullOrEmpty(newFormula) ? "0" : newFormula;
		UpdateView();
	}

	public void OnNameChanged() {
		OnNameChanged(nameText.Text);
	}

	public void OnNameChanged(string newName) {
		if (newName == cell.Name)
			return;

		cell.TryNameChange(newName);
		UpdateView();
	}

	private void OnDeleteCellPressed() {
		((ConfirmationDialog)GetNode("DeleteCellDialog")).PopupCentered();
	}

	private void DeleteCellConfirmed() {
		DeleteCell?.Invoke(cell);
	}

	public void Delete() {
		cell.ValueChanged -= CellValueChanged;
	}

	public override void _Input(InputEvent @event) {
		if (dragging) {
			if (@event is InputEventMouseMotion eventMouseMotion) {
				RectPosition = eventMouseMotion.Position - dragOffset;
				(GetParent() as CanvasItem).Update();
			} else if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.Pressed) {
				dragging = false;
			}
		} else {
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
				Rect2 dragArea = dragAreaPanel.GetRect();
				dragArea.Position += RectPosition;

				if (dragArea.HasPoint(eventMouseButton.Position)) {
					dragging = true;
					dragOffset = eventMouseButton.Position - RectPosition;
				}
			}
		}
	}
}

