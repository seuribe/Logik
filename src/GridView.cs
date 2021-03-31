using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class GridView : Control {

	private static readonly PackedScene cellScene = GD.Load<PackedScene>("res://scenes/cell.tscn");
	private static readonly CellIndex cellIndex = new CellIndex();
	private static readonly Dictionary<Cell, CellView> views = new Dictionary<Cell, CellView>();
	
	private static readonly Color ReferenceColor = new Color(184f/256, 89f/256, 2f/256);
	private static readonly Color ErrorColor = new Color(0.8f, 0, 0);

	private static readonly Vector2 CellPositionIncrement = new Vector2(40, 40);
	private static readonly Vector2 MaxCellPosition = CellPositionIncrement * 10;
	private Vector2 nextCellPosition = new Vector2(0, 0);

	public void AddCell(Vector2 position) {
		var cell = cellIndex.CreateCell();
		var sceneInstance = cellScene.Instance();
		var cellView = (sceneInstance as CellView);

		views.Add(cell, cellView);

		AddChild(cellView);
		cellView.RectPosition = position;
		cellView.SetCell(cell);
		cell.ValueChanged += (c) => {
			Update();
		};
	}

	private void OnAddCellPressed() {
		nextCellPosition += CellPositionIncrement;
		if (nextCellPosition == MaxCellPosition) {
			nextCellPosition.x = 0;
			nextCellPosition.y = 0;
		}
		AddCell(nextCellPosition);
	}

	public override void _Draw() {
		foreach (var cell in views.Keys) {
			foreach (var other in cell.Referenced) {
				var fromView = views[cell];
				var toView = views[other];
				DrawLine(fromView.ConnectorStart, toView.ConnectorEnd, other.Error ? ErrorColor : ReferenceColor);
			}
		}
	}
}
