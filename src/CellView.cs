using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class CellView : Control {
	private Label valueLabel;
	private TextEdit nameText;
	private TextEdit formulaText;
	private Panel dragAreaPanel;
	private Panel mainPanel;
	private Cell cell;

	private bool dragging = false;
	private Vector2 dragOffset;

	private static readonly StyleBoxFlat StyleError = GD.Load<StyleBoxFlat>("res://styles/cell_error.tres");
	private static readonly StyleBoxFlat StyleNormal = GD.Load<StyleBoxFlat>("res://styles/cell_normal.tres");


	public override void _Ready() {
		valueLabel = (Label)GetNode("Panel/ValueLabel");
		nameText = (TextEdit)GetNode("Panel/NameText");
		formulaText = (TextEdit)GetNode("Panel/FormulaText");
		dragAreaPanel = (Panel)GetNode("Panel/DragArea");
		mainPanel = (Panel)GetNode("Panel");
	}

	public void SetCell(Cell cell) {
		if (this.cell != null)
			this.cell.ValueChanged -= OnCellValueChange;

		this.cell = cell;
		nameText.Text = cell.Id;
		formulaText.Text = cell.Formula;

		OnCellValueChange(cell);

		cell.ValueChanged += OnCellValueChange;
	}

	private void OnCellValueChange(Cell cell) {
		valueLabel.Text = cell.Value;
		GD.Print($"Value for {cell.Id} changed to {cell.Value}. Number of referenced Cells: {cell.Referenced.Count}");
		mainPanel.Set("custom_styles/panel", (cell.Error) ? StyleError : StyleNormal);
		Update();
	}

	public void OnFormulaChanged() {
		cell.Formula = formulaText.Text;
		valueLabel.Text = cell.Value;
	}

	public override void _Input(InputEvent @event)
	{
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

