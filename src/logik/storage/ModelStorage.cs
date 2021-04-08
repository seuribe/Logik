using System;
using System.IO;
using System.Text.Json;
using Logik.Core;
using Logik.Core.Formula;

namespace Logik.Storage {

    public class ModelStorage {
        private const string CellNameProperty = "name";
        private const string CellFormulaProperty = "formula";

        private const string CellsArrayProperty = "cells";
        private const string EvaluatorProperty = "evaluator";

        public void Save(Model model, string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fs)) {
                    writer.WriteStartObject();
                    writer.WriteString(EvaluatorProperty, model.Evaluator.Type);
                    writer.WriteStartArray(CellsArrayProperty);
                    foreach (var cell in model.GetCells()) {
                        writer.WriteStartObject();
                        writer.WriteString(CellNameProperty, cell.Name);
                        writer.WriteString(CellFormulaProperty, cell.Formula);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                    writer.WriteEndObject();
                    writer.Flush();
                }
            }
        }

        public Model Load(string filename) {
            byte[] data = File.ReadAllBytes(filename);
            using (JsonDocument doc = JsonDocument.Parse(data)) {
                var root = doc.RootElement;
                var evaluatorType = root.GetProperty(EvaluatorProperty).GetString();

                Model model = new Model(EvaluatorProvider.GetEvaluator(evaluatorType));

                var cells = root.GetProperty(CellsArrayProperty);
                foreach (var jsonCell in cells.EnumerateArray()) {
                    var name = jsonCell.GetProperty(CellNameProperty).GetString();
                    var formula = jsonCell.GetProperty(CellFormulaProperty).GetString();
                    model.CreateCell(name, formula);
                }
                return model;
            }
        }
    }
}
