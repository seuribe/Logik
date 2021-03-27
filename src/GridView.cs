using Godot;
using Logik.Core;
using System;
using System.Collections.Generic;

public class GridView : Control
{

	private PackedScene cellScene;
	private CellIndex cellIndex;
	private static Dictionary<Cell, CellView> views = new Dictionary<Cell, CellView>();

	public override void _Ready()
	{
		cellScene = GD.Load<PackedScene>("res://scenes/cell.tscn");
		cellIndex = new CellIndex();
	}

	public void AddCell() {
		var cell = cellIndex.CreateCell();
		var cellView = cellScene.Instance();
		AddChild(cellView);
		(cellView as CellView).SetCell(cell);
		views.Add(cell, (cellView as CellView));
	}

	private void OnAddCellPressed() {
		AddCell();
	}

	public override void _Draw() {
/*
		foreach (var other in cell.Referenced) {
			var view = views[other];
			DrawLine(RectPosition, view.RectPosition, new Color(0, 0, 1));
			GD.Print($"Draw Line from {cell.Id} ({RectPosition}) to {other.Id} ({view.RectPosition})");
		}
*/
	}

}
