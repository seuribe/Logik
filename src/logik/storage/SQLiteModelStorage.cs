using System;
using System.Collections.Generic;
using System.Linq;
using Logik.Core;
using SQLite;

namespace Logik.Storage {
    public class SQLiteStorageConstants {
        public const string TypeNumeric = "numeric";
        public const string TypeTabulaer = "tabular";
    }

    [Table("cells")]
    internal class CellData {

        [PrimaryKey]
        public string Name { get; set; }
        public string Formula { get; set; }
        public string Type { get; set; }
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
    }

    [Table("model")]
    internal class ModelData {
        public string Evaluator { get; set; }
    }

    public class SQLiteModelStorage : SQLiteStorageConstants, IDisposable {
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
            db.DeleteAll<CellData>();
            db.InsertAll(
                model.GetCells().Select( cell => new CellData() { Name = cell.Name, Formula = cell.Formula } )
                );
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

            var query = db.Table<CellData>();
            var data = query.ToList();
            foreach (var celldata in data) {
                model.CreateCell(celldata.Name, celldata.Formula);
            }
            return model;
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
