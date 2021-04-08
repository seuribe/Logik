using System;
using System.IO;
using System.Text.Json;
using Logik.Core;
using Logik.Core.Formula;

namespace Logik.Storage {

    public class ModelStorage {
        private const string CellNameProperty = "name";
        private const string CellFormulaProperty = "formula";
        private const string CellArrayProperty = "cells";

        public void Save(Model model, string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fs)) {
                    writer.WriteStartObject();
                    writer.WriteStartArray(CellArrayProperty);
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
                Model model = new Model(new TreeEvaluator());
                var root = doc.RootElement;
                var cells = root.GetProperty(CellArrayProperty);
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
