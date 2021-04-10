using Godot;
using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using System;
using System.Linq;
using System.Collections.Generic;

public class ModelView : Control {

	private static readonly PackedScene cellScene = GD.Load<PackedScene>("res://scenes/cell.tscn");
	private static readonly Dictionary<Cell, CellView> views = new Dictionary<Cell, CellView>();
	
	private static readonly Color ReferenceColor = new Color(184f/256, 89f/256, 2f/256);
	private static readonly Color ErrorColor = new Color(0.8f, 0, 0);

	private static readonly Vector2 CellPositionIncrement = new Vector2(40, 40);
	private static readonly Vector2 MaxNewCellPosition = CellPositionIncrement * 10;
	private Vector2 nextCellPosition = Vector2.Zero;

	private Model model;


	public override void _Ready() {
		SetModel(new Model(new TreeEvaluator()));
		CreateCellViews();
	}

	private void CreateCellViews(Dictionary<string, Vector2> viewPositions = null) {
		RemoveAllViews();
		foreach (var cell in model.GetCells())
			AddCellView(cell, viewPositions?[cell.Name] ?? GetNextCellPosition());

		Update();
	}

	public void SetModel(Model model) {
		this.model = model;
		model.Evaluate();
		model.UpdateReferences();
	}

	private void RemoveAllViews() {
		foreach (var view in views.Values)
			RemoveChild(view);

		views.Clear();
	}

	public void AddCellView(Cell cell, Vector2 position) {
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
	
	private Vector2 GetNextCellPosition() {
		nextCellPosition += CellPositionIncrement;
		if (nextCellPosition >= MaxNewCellPosition)
			nextCellPosition = Vector2.Zero;

		return nextCellPosition;
	}

	private void OnAddCellPressed() {
		AddCellView(model.CreateCell(), GetNextCellPosition());
	}

	private void OnLoadButtonPressed() {
		var fd = (FileDialog)GetNode("OpenDialog");
		fd.PopupCentered();
	}

	private void OnSaveButtonPressed() {
		var fd = (FileDialog)GetNode("SaveDialog");
		fd.PopupCentered();
	}

	private void OnLoadFileSelected(string filename) {
		using (SQLiteModelStorage storage = new SQLiteModelStorage(filename)) {
			SetModel(storage.LoadModel());
			CreateCellViews(storage.LoadViews());
		}
	}

	private void OnSaveFileSelected(string filename) {
		using (SQLiteModelStorage storage = new SQLiteModelStorage(filename)) {
			storage.StoreModel(model);
			storage.StoreViews(views.ToDictionary( kv => kv.Key.Name, kv => kv.Value.RectPosition ));
		}
	}

	public override void _Draw() {
		foreach (var cell in views.Keys) {
			foreach (var other in cell.references) {
				DrawReference(cell, other);
			}
		}
	}

	private void DrawReference(Cell cell, Cell other) {
		var from = views[cell].ConnectorBottom;
		var to = views[other].ConnectorTop;
		var mid = (from + to) / 2;
		var step1 = new Vector2(from.x, mid.y);
		var step2 = new Vector2(to.x, mid.y);
		
		DrawLine(from, step1, other.Error ? ErrorColor : ReferenceColor);
		DrawLine(step1, step2, other.Error ? ErrorColor : ReferenceColor);
		DrawLine(step2, to, other.Error ? ErrorColor : ReferenceColor);

	}
}
