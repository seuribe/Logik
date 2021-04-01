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

	public Vector2 ConnectorStart { get => RectPosition + formulaText.RectPosition + (formulaText.RectSize/2); }
	public Vector2 ConnectorEnd { get => RectPosition + valueLabel.RectPosition + (valueLabel.RectSize/2); }

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
			this.cell.ContentChanged -= CellContentChanged;

		this.cell = cell;

		CellContentChanged(cell);

		cell.ContentChanged += CellContentChanged;
	}

	private void CellContentChanged(Cell cell) {
		valueLabel.Text = cell.Value;
		nameText.Text = cell.Id;
		mainPanel.Set("custom_styles/panel", (cell.Error) ? StyleError : StyleNormal);
		Update();
	}

	public void OnFormulaChanged() {
		cell.Formula = formulaText.Text;
		valueLabel.Text = cell.Value;
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

