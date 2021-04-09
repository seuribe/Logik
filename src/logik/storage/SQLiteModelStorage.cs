﻿using System;
using System.Collections.Generic;
using System.Linq;
using Logik.Core;
using SQLite;

namespace Logik.Storage {

    [Table("cells")]
    internal class CellData {

        [PrimaryKey]
        public string Name { get; set; }
        public string Formula { get; set; }
    }

    [Table("cellviews")]
    internal class CellViewData {

        [PrimaryKey]
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
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
        }

        public void StoreModel(Model model) {
            db.DeleteAll<ModelData>();
            db.Insert(new ModelData { Evaluator = model.Evaluator.Type });
            db.DeleteAll<CellData>();
            db.InsertAll(
                model.GetCells().Select( cell => new CellData() { Name = cell.Name, Formula = cell.Formula } )
                );
        }

        public void StoreViews(Dictionary<string, Godot.Vector2> viewPositions) {
            db.DeleteAll<CellViewData>();
            db.InsertAll(
                viewPositions.Select( kv => new CellViewData() {
                    Name = kv.Key,
                    X = kv.Value.x,
                    Y = kv.Value.y} )
                );
        }

        public Model LoadModel() {
            var evaluatorType = db.Table<ModelData>().First().Evaluator;
            var model = new Model(EvaluatorProvider.GetEvaluator(evaluatorType));

            var query = db.Table<CellData>();
            var data = query.ToList();
            foreach (var celldata in data) {
                model.CreateCell(celldata.Name, celldata.Formula);
            }
            return model;
        }

        public Dictionary<string, Godot.Vector2> LoadViews() {
            var cellViews = new Dictionary<string, Godot.Vector2>();

            var query = db.Table<CellViewData>();
            var data = query.ToList();
            foreach (var cvd in data)
                cellViews.Add(cvd.Name, new Godot.Vector2(cvd.X, cvd.Y));

            return cellViews;
        }

        public void Dispose() {
            db.Close();
        }
    }
}
