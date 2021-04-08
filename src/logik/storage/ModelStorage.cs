using System;
using System.IO;
using System.Text.Json;
using Logik.Core;
using Logik.Core.Formula;

namespace Logik.Storage {

    public class ModelStorage {

        public void Save(Model model, string filename) {
            using (FileStream fs = new FileStream(filename, FileMode.Create)) {
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fs)) {
                    writer.WriteStartObject();
                    writer.WriteStartArray("cells");
                    foreach (var cell in model.GetCells()) {
                        writer.WriteStartObject();
                        writer.WriteString("name", cell.Name);
                        writer.WriteString("formula", cell.Formula);
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
                var cells = root.GetProperty("cells");
                foreach (var jsonCell in cells.EnumerateArray()) {
                    var name = jsonCell.GetProperty("name").GetString();
                    var formula = jsonCell.GetProperty("formula").GetString();
                    var cell = model.CreateCell();
                    cell.Formula = formula;
                    cell.TryNameChange(name);
                }
                return model;
            }
        }
    }
}
