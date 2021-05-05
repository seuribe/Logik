using Godot;
using Logik.Core;
using Logik.Core.Formula;
using Logik.Storage;
using Logik.View;
using System;
using System.Linq;
using System.Collections.Generic;

public class ModelView : Control {

	private static readonly PackedScene cellScene = GD.Load<PackedScene>("res://scenes/cell.tscn");
	private static readonly PackedScene tableScene = GD.Load<PackedScene>("res://scenes/TableCell.tscn");
	private static readonly Dictionary<ICell, CellView> views = new Dictionary<ICell, CellView>();
	private static readonly Dictionary<TabularCell, TableCellView> tviews = new Dictionary<TabularCell, TableCellView>();
	
	private static readonly Color FocusReferenceColor = new Color(184f/256, 89f/256, 2f/256);
	private static readonly Color ReferenceColor = new Color(0.4f, 0.4f, 0.4f);
	private static readonly Color ErrorColor = new Color(0.8f, 0, 0);

	private static readonly Vector2 CellPositionIncrement = new Vector2(40, 40);
	private static readonly Vector2 MaxNewCellPosition = CellPositionIncrement * 10;
	private Vector2 nextCellPosition = Vector2.Zero;

	private Model model;

	public static bool SnapToGrid { get; private set; }
	public static bool WorkMode { get; private set; }
	public static bool EditMode { get => !WorkMode; }

	public override void _Ready() {
		SetModel(new Model());
		CreateCellViews();
	}

	private void CreateCellViews(Dictionary<string, CellViewState> viewPositions = null) {
		RemoveAllViews();
		foreach (var cell in model.GetCells()) {
			if (viewPositions != null) {
				AddCellView(cell, viewPositions[cell.Name]);
			} else {
				AddCellView(cell, GetNextCellPosition());
			}
		}

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

	public void AddTableView(TabularCell tcell, Vector2 position) {
		var tableView = (tableScene.Instance() as TableCellView);
		tviews.Add(tcell, tableView);
		AddChild(tableView);

		tableView.RectPosition = position;
		tableView.SetCell(tcell);
	}

	public void AddCellView(NumericCell cell, Vector2 position) {
		AddCellView(cell, new CellViewState(position));
	}

	public void AddCellView(NumericCell cell, CellViewState viewState) {
		var cellView = (cellScene.Instance() as CellView);
		views.Add(cell, cellView);
		AddChild(cellView);

		cellView.RectPosition = viewState.position;
		cellView.InputOnly = viewState.inputOnly;
		cellView.SetCell(cell);
		cell.ValueChanged += (c) => {
			Update();
		};
		cellView.DeleteCell += DeleteCell;
		cellView.PositionChanged += CellPositionChanged;
	}

	private void CellPositionChanged(ICell cell) {
		Update();
	}

	private void DeleteCell(ICell cell) {
		var view = views[cell];
		view.Delete();
		view.DeleteCell -= DeleteCell;
		view.PositionChanged -= CellPositionChanged;
		view.QueueFree();
		views.Remove(cell);
		model.DeleteCell(cell);
		Update();
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
	private void OnAddTablePressed() {
		AddTableView(model.CreateTable(), GetNextCellPosition());
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
			storage.StoreViews(views.ToDictionary( kv => kv.Key.Name, kv => new CellViewState(kv.Value)));
		}
	}

	public override void _Draw() {
		if (!WorkMode)
			DrawReferences();

		if (SnapToGrid)
			DrawGrid();
	}

	private void DrawGrid() {
		var boundaries = GetViewportRect().Size;
		for (int y = 0 ; y < boundaries.y ; y += (int)Grid.GridSize.x) {
			for (int x = 0 ; x < boundaries.x; x += (int)Grid.GridSize.y) {
				DrawLine(new Vector2(x - 5, y), new Vector2(x + 5, y), ReferenceColor);
				DrawLine(new Vector2(x, y - 5), new Vector2(x, y + 5), ReferenceColor);
			}
		}
	}

	private void DrawReferences() {
		foreach (var cell in views.Keys) {
			if (cell is NumericCell ncell) {
				foreach (var other in ncell.references) {
					DrawReference(other, ncell);
				}
			}
		}
	}

	private void DrawReference(NumericCell from, NumericCell to) {
		var fromView = views[from];
		var toView = views[to];
		var start = fromView.ConnectorTop;
		var end = toView.ConnectorBottom;
		var mid = (start + end) / 2;
		var step1 = new Vector2(start.x, mid.y);
		var step2 = new Vector2(end.x, mid.y);
		var hover = (fromView.Hover || toView.Hover);
		var color = from.Error ? ErrorColor :
					hover ? FocusReferenceColor : ReferenceColor;
		var width = hover ? 2 : 1;
		
		DrawPolyline(new Vector2[]{start, step1, step2, end}, color, width);
		DrawCircle(end, 5, color);
	}

	public override void _Input(InputEvent @event) {
		if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.Pressed) {
			GetFocusOwner()?.ReleaseFocus();
		}
	}

	private void SwitchWorkMode() {
		foreach (var cell in views.Keys) {
			var view = views[cell];
			view.Visible = EditMode || CellIsInput(cell) || CellIsOutput(cell);
			view.WorkMode = WorkMode;
		}
	}

	private bool CellIsInput(ICell cell) {
		return views[cell].InputOnly;
	}

	private bool CellIsOutput(ICell cell) {
		return (cell as NumericCell).referencedBy.Count == 0;
	}

	private void OnSnapToGridToggle(bool pressed) {
		SnapToGrid = pressed;
		Update();
	}

	private void OnWorkModeToggle(bool pressed) {
		WorkMode = pressed;
		SwitchWorkMode();
		Update();
	}
}

