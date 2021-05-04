using System;
using System.IO;
using System.Text.Json;
using Logik.Core;
using Logik.Core.Formula;

namespace Logik.Storage {

    public class JsonConstants {
        public const string CellNameProperty = "name";
        public const string CellFormulaProperty = "formula";

        public const string CellsArrayProperty = "cells";
        public const string EvaluatorProperty = "evaluator";
    }

    public class JsonModelWriter : JsonConstants, IDisposable {
        private readonly Utf8JsonWriter writer;

        public JsonModelWriter(Stream stream) {
            this.writer = new Utf8JsonWriter(stream);
        }
        
        public void WriteModel(Model model) {
            WriteModelStart(model);
            WriteCells(model);
            WriteModelEnd();
        }

        private void WriteModelStart(Model model) {
            writer.WriteStartObject();
            writer.WriteString(EvaluatorProperty, model.EvaluatorType);
        }
        
        private void WriteModelEnd() {
            writer.WriteEndObject();
            writer.Flush();
        }

        private void WriteCells(Model model) {
            writer.WriteStartArray(CellsArrayProperty);
            foreach (var cell in model.GetCells()) {
                WriteCell(cell);
            }
            writer.WriteEndArray();
        }

        private void WriteCell(NumericCell cell) {
            writer.WriteStartObject();
            writer.WriteString(CellNameProperty, cell.Name);
            writer.WriteString(CellFormulaProperty, cell.Formula);
            writer.WriteEndObject();
        }

        public void Dispose() {
            writer.Dispose();
        }
    }

    public class JsonModelReader : JsonConstants, IDisposable {
        private readonly JsonDocument json;
        public JsonModelReader(Stream stream) {
            json = JsonDocument.Parse(stream);
        }

        public Model ReadModel() {
            var root = json.RootElement;
            Model model = CreateModel(root);
            ReadCells(root, model);
            model.Evaluate();
            return model;
        }

        private static Model CreateModel(JsonElement root) {
            return new Model();
        }

        private static void ReadCells(JsonElement root, Model model) {
            var cells = root.GetProperty(CellsArrayProperty);
            foreach (var jsonCell in cells.EnumerateArray()) {
                ReadCell(model, jsonCell);
            }
        }

        private static void ReadCell(Model model, JsonElement jsonCell) {
            var name = jsonCell.GetProperty(CellNameProperty).GetString();
            var formula = jsonCell.GetProperty(CellFormulaProperty).GetString();
            model.CreateCell(name, formula);
        }

        public void Dispose() {
            json.Dispose();
        }
    }

    public class JsonModelStorage {
 
        public static void Save(Model model, string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                using (JsonModelWriter writer = new JsonModelWriter(fs)) {
                    writer.WriteModel(model);
                }
            }
        }

        public static Model Load(string filename) {
            using (FileStream stream = File.OpenRead(filename)) {
                using (JsonModelReader reader = new JsonModelReader(stream)) {
                    return reader.ReadModel();
                }
            }
        }
    }
}
