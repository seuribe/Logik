using System;
using System.Collections.Generic;
using System.Linq;
using Logik.Core;
using SQLite;

namespace Logik.Storage {

    public class Types {
        public const string Numeric = "numeric";
        public const string Tabular = "tabular";
    }

    [Table("cells")]
    internal class CellData {

        [PrimaryKey]
        public string Name { get; set; }
        public string Formula { get; set; }
        public string Type { get; set; }

        public static CellData FromCell(ICell cell) {
            return new CellData {
                Name = cell.Name,
                Formula = (cell is NumericCell ncell) ? ncell.Formula : "",
                Type = (cell is NumericCell) ? Types.Numeric :
                   (cell is TabularCell) ? Types.Tabular :
                   throw new Exception ("Unsupported Cell type")
            };
        }
    }

    [Table("cellviews")]
    internal class CellViewData {

        [PrimaryKey]
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public bool InputOnly { get; set; }
    }

    [Table("tablecells")]
    internal class TableCellData {
        [Indexed]
        public string CellName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Data { get; set; }

        public static IEnumerable<TableCellData> From(TabularCell tcell) {
            var data = new List<TableCellData>();
            for (int row = 0 ; row < tcell.Rows ; row++)
                for (int column = 0 ; column < tcell.Columns ; column++)
                    data.Add(new TableCellData {
                        CellName = tcell.Name,
                        Row = row,
                        Column = column,
                        Data = tcell[row, column].ToString()
                    });
            return data;
        }
    }

    [Table("model")]
    internal class ModelData {
        public string Evaluator { get; set; }
    }

    public class SQLiteModelStorage : IDisposable {
        private readonly SQLiteConnection db;

        public SQLiteModelStorage(string filename) {
            db = new SQLiteConnection(filename);
            db.CreateTable<CellData>();
            db.CreateTable<CellViewData>();
            db.CreateTable<ModelData>();
            db.CreateTable<TableCellData>();
        }

        public void StoreModel(Model model) {
            db.DeleteAll<ModelData>();
            db.Insert(new ModelData { Evaluator = model.EvaluatorType });
            var cells = model.GetCells();
            StoreCells(cells);
            StoreTabularData(cells);
        }

        private void StoreCells(IEnumerable<ICell> cells) {
            db.DeleteAll<CellData>();
            db.InsertAll(cells.Select(CellData.FromCell));
        }

        private void StoreTabularData(IEnumerable<ICell> cells) {
            var tcells = cells.Where( cell => cell is TabularCell).Select( cell => cell as TabularCell);
            db.DeleteAll<TableCellData>();
            foreach (var tcell in tcells) {
                db.InsertAll(TableCellData.From(tcell));
            }
        }

        public void StoreViews(Dictionary<string, CellViewState> viewPositions) {
            db.DeleteAll<CellViewData>();
            db.InsertAll(
                viewPositions.Select( kv => new CellViewData() {
                    Name = kv.Key,
                    X = kv.Value.position.x,
                    Y = kv.Value.position.y,
                    InputOnly = kv.Value.inputOnly} )
                );
        }

        public Model LoadModel() {
            var evaluatorType = db.Table<ModelData>().First().Evaluator;
            var model = new Model();

            LoadCells(model);
            return model;
        }

        private void LoadCells(Model model) {
            var query = db.Table<CellData>();
            var data = query.ToList();
            foreach (var celldata in data) {
                if (celldata.Type == Types.Numeric)
                    model.CreateCell(celldata.Name, celldata.Formula);
            }
        }

        public Dictionary<string, CellViewState> LoadViews() {
            var cellViews = new Dictionary<string, CellViewState>();

            var query = db.Table<CellViewData>();
            var data = query.ToList();
            foreach (var cvd in data)
                cellViews.Add(cvd.Name, new CellViewState { position = new Godot.Vector2(cvd.X, cvd.Y), inputOnly = cvd.InputOnly } );

            return cellViews;
        }

        public void Dispose() {
            db.Close();
        }
    }
}
