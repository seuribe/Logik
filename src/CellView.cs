using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class CellView : Control
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	private Label valueLabel;
	private TextEdit nameText;
	private TextEdit formulaText;
	private Panel dragAreaPanel;
	private Cell cell;
	private Rect2 dragArea;

	public string formula;
	public string value {
		get { return formula; }
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		valueLabel = (Label)GetNode("Panel/ValueLabel");
		nameText = (TextEdit)GetNode("Panel/NameText");
		formulaText = (TextEdit)GetNode("Panel/FormulaText");
		dragAreaPanel = (Panel)GetNode("Panel/DragArea");

		UpdateDragArea();
	}

	public void SetCell(Cell cell) {
		this.cell = cell;
		cell.ValueChanged += (_) => {
			valueLabel.Text = cell.Value;
			GD.Print($"Value for {cell.Id} changed to {cell.Value}. Number of referenced Cells: {cell.Referenced.Count}");
		};
		nameText.Text = cell.Id;
	}

	private void UpdateDragArea() {
		dragArea = dragAreaPanel.GetRect();
		dragArea.Position += RectPosition;
	}

	public void onFormulaChanged() {
		formula = formulaText.Text;
		cell.Formula = formula;
		valueLabel.Text = cell.Value;
	}

	public string evalFormula(string formula) {
		return formula;
	}

	private bool dragging = false;
	private Vector2 dragOffset;
	
	public override void _Input(InputEvent @event)
	{
		if (dragging) {
			if (@event is InputEventMouseMotion eventMouseMotion) {
				RectPosition = eventMouseMotion.Position - dragOffset;
			} else if (@event is InputEventMouseButton eventMouseButton && !eventMouseButton.Pressed) {
				dragging = false;
				UpdateDragArea();
			}
		} else {
			if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
				if (dragArea.HasPoint(eventMouseButton.Position)) {
					dragging = true;
					dragOffset = eventMouseButton.Position - RectPosition;
				}
			}
		}
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

