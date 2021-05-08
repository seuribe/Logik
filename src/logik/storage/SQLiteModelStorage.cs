using System;
using System.Collections.Generic;
using System.Linq;
using Logik.Core;
using SQLite;

namespace Logik.Storage {

    [Table("cells")]
    internal class NumericCellData {

        [PrimaryKey]
        public string Name { get; set; }
        public string Formula { get; set; }

        public static NumericCellData FromCell(NumericCell cell) {
            return new NumericCellData {
                Name = cell.Name,
                Formula = cell.Formula
            };
        }
    }
    
    [Table("tables")]

    internal class TabularCellData {
        public string Name { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public static TabularCellData From(TabularCell tcell) {
            return new TabularCellData {
                Name = tcell.Name,
                Rows = tcell.Rows,
                Columns = tcell.Columns
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

    [Table("tablevalues")]
    internal class TablularCellGridData {
        [Indexed]
        public string CellName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Value { get; set; }

        public static IEnumerable<TablularCellGridData> From(TabularCell tcell) {
            var data = new List<TablularCellGridData>();
            for (int row = 0 ; row < tcell.Rows ; row++)
                for (int column = 0 ; column < tcell.Columns ; column++)
                    data.Add(new TablularCellGridData {
                        CellName = tcell.Name,
                        Row = row,
                        Column = column,
                        Value = tcell[row, column].ToString()
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
            db.CreateTable<NumericCellData>();
            db.CreateTable<CellViewData>();
            db.CreateTable<ModelData>();
            db.CreateTable<TabularCellData>();
            db.CreateTable<TablularCellGridData>();
        }

        public void StoreModel(Model model) {
            db.DeleteAll<ModelData>();
            db.Insert(new ModelData { Evaluator = model.EvaluatorType });
            var cells = model.GetCells();
            StoreCells(cells);
        }

        private void StoreCells(IEnumerable<ICell> cells) {
            db.DeleteAll<NumericCellData>();
            var ncells = cells.Where( cell => cell is NumericCell).Select( cell => cell as NumericCell);
            db.InsertAll(ncells.Select(NumericCellData.FromCell));
            var tcells = cells.Where( cell => cell is TabularCell).Select( cell => cell as TabularCell);
            db.DeleteAll<TabularCellData>();
            db.DeleteAll<TablularCellGridData>();
            foreach (var tcell in tcells) {
                db.Insert(TabularCellData.From(tcell));
                db.InsertAll(TablularCellGridData.From(tcell));
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
            var model = new Model();

            LoadCells(model);
            return model;
        }

        private void LoadCells(Model model) {
            LoadNumericCells(model);
            LoadTabularCells(model);
        }

        private void LoadTabularCells(Model model) {
            var tquery = db.Table<TabularCellData>();
            var tdata = tquery.ToList();
            foreach (var tcelldata in tdata) {
                var dataquery = db.Table<TablularCellGridData>().Where(tcd => tcd.CellName == tcelldata.Name);
                var gridCellData = dataquery.Select(
                    tcd => new GridCellData { Column = tcd.Column, Row = tcd.Row, Value = float.Parse(tcd.Value) }
                    );
//                var tt = gridCellData.ToList();
                model.CreateTable(tcelldata.Name, tcelldata.Rows, tcelldata.Columns, gridCellData);
            }
        }

        private void LoadNumericCells(Model model) {
            var query = db.Table<NumericCellData>();
            var data = query.ToList();
            foreach (var celldata in data) {
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
