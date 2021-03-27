using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class GridView : Control {

	private static PackedScene cellScene = GD.Load<PackedScene>("res://scenes/cell.tscn");
	private static CellIndex cellIndex = new CellIndex();
	private static Dictionary<Cell, CellView> views = new Dictionary<Cell, CellView>();
	
	public void AddCell() {
		var cell = cellIndex.CreateCell();
		var cellView = cellScene.Instance();
		AddChild(cellView);
		(cellView as CellView).SetCell(cell);
		views.Add(cell, (cellView as CellView));

		cell.ValueChanged += (c) => {
			Update();
		};
	}

	private void OnAddCellPressed() {
		AddCell();
	}

	public override void _Draw() {
		foreach (var cell in views.Keys) {
			foreach (var other in cell.Referenced) {
				var fromView = views[cell];
				var toView = views[other];
				DrawLine(fromView.RectPosition, toView.RectPosition, new Color(0, 0, 1));
			}
		}
	}

}
